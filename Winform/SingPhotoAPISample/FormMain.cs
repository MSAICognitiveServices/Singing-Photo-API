//
//  Copyright(c) Microsoft.All rights reserved.
//  Licensed under the MIT license.
//
//  Microsoft Cognitive Services(formerly Project Oxford) : https://www.microsoft.com/cognitive-services
//
//  Microsoft Cognitive Services(formerly Project Oxford) GitHub:
//  https://github.com/Microsoft/ProjectOxford-ClientSDK
//
//  Copyright(c) Microsoft Corporation
//  All rights reserved.
//
//  MIT License:
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebAPIClient
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        public string filePath = string.Empty;

        private void btn_SelectFile_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.openFileDialog1.FileNames != null && this.openFileDialog1.FileNames.Length > 0)
                {
                    foreach (var file in this.openFileDialog1.FileNames)
                    {
                        this.textBox_Path.Text = file;
                        filePath = System.IO.Path.GetDirectoryName(file);
                    }
                }
            }
        }
        private void btRequest_Click(object sender, EventArgs e)
        {
            this.txtResponse.Text = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                using (var content = new MultipartFormDataContent())
                {
                    var formDatas = this.GetFormDataByteArrayContent(this.GetNameValueCollection());
                    var files = this.GetFileByteArrayContent(this.GetHashSet(this.textBox_Path.Text));
                    Action<List<ByteArrayContent>> act_formdata = (dataContents) =>
                    {
                        foreach (var byteArrayContent in dataContents)
                        {
                            content.Add(byteArrayContent, "code");
                        }
                    };
                    Action<List<ByteArrayContent>> act_file = (dataContents) =>
                    {
                        for(int i = 0;i<dataContents.Count;i++)
                        {
                            content.Add(dataContents[i], "imageFile", filePath);
                        }
                    };

                    act_formdata(formDatas);
                    act_file(files);
                    try
                    {
                        var result = client.PostAsync("http://singingphoto.com/songbot/requestsong", content).Result;
                        this.txtResponse.Text = result.Content.ReadAsStringAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        this.txtResponse.Text = ex.ToString();
                    }
                }
            }
        }
        private List<ByteArrayContent> GetFileByteArrayContent(HashSet<string> files)
        {
            List<ByteArrayContent> list = new List<ByteArrayContent>();
            foreach (var file in files)
            {
                var fileContent = new ByteArrayContent(File.ReadAllBytes(file));
                ContentDispositionHeaderValue dispositionHeader = new ContentDispositionHeaderValue("file");
                dispositionHeader.DispositionType = "file";
                dispositionHeader.Name = "imageFile";
                dispositionHeader.FileName = Path.GetFileName(file);              
                fileContent.Headers.ContentDisposition = dispositionHeader;
                list.Add(fileContent);
            }
            return list;
        }
        private List<ByteArrayContent> GetFormDataByteArrayContent(NameValueCollection collection)
        {
            List<ByteArrayContent> list = new List<ByteArrayContent>();
            foreach (var key in collection.AllKeys)
            {
                var dataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(collection[key]));
                dataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = key
                };
                list.Add(dataContent);
            }
            return list;
        }
        private NameValueCollection GetNameValueCollection(DataGridView gv)
        {
            NameValueCollection collection = new NameValueCollection();
            var rows = gv.Rows;
            foreach (DataGridViewRow row in rows)
            {
                try
                {
                    if (row.Cells[0].Value != null)
                    {
                        collection.Add(row.Cells[0].Value.ToString(),
                            row.Cells[1].Value == null ? string.Empty : row.Cells[1].Value.ToString());
                    }
                }
                catch { }
            }
            return collection;
        }
        private NameValueCollection GetNameValueCollection()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("code", "5a96a50f-4a47-4760");
            return collection;
        }
        private HashSet<string> GetHashSet(string path)
        {
            HashSet<string> hash = new HashSet<string>();
            hash.Add(path);
            return hash;
        }
        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private Point mPoint = new Point();
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint.X = e.X;
            mPoint.Y = e.Y;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point myPosittion = MousePosition;
                myPosittion.Offset(-mPoint.X, -mPoint.Y);
                Location = myPosittion;
            }
        }
    }
}
