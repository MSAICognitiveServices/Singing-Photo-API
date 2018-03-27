'''
Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license.
Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
Microsoft Cognitive Services (formerly Project Oxford) GitHub:
https://github.com/Microsoft/ProjectOxford-ClientSDK
Copyright (c) Microsoft Corporation
All rights reserved.
MIT License:
Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:
The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'''

import sys, getopt, os
import codecs, urllib2
import mimetypes, mimetools
import itertools
#
# Sample for Singing Photo API
#

def usage():
    print(
        """
============== Singing Photo API Sample ==============
Usage:
python sample.py -i <InputImage> [-o <OutputSong>]

-h: Display help information
-i or --iInputImage: Input image file path.
-o or --oOutputSong: Ouput song and analysis results.
======================================================
        """
    )

def saveFile(filePath, fileContent):
    fh = codecs.open(filePath, "w", "UTF-8")
    fh.write(fileContent.decode("utf-8"))
    fh.close()

if len(sys.argv) == 1:
    usage()
    sys.exit()

inputImage = ""
outputSong = ""
try:
    opts, args = getopt.getopt(sys.argv[1:], "hi:o:", ["ifile=","ofile="])
except getopt.GetoptError:
    usage()
    sys.exit(2)
for opt, arg in opts:
    if opt == '-h':
        usage()
        sys.exit(2)
    elif opt in ("-i", "--iInputImage"):
        inputImage = arg
    elif opt in ("-o", "--oOutputSong"):
        outputSong = arg
        
# Request parameters
os.system("chcp 65001")
os.system("cls")
_url = 'http://singingphoto.com/songbot/requestsong'
code = '5a96a50f-4a47-4760'
image = open(inputImage, 'rb')
mimetype = mimetypes.guess_type(image.name)[0] or 'application/octet-stream'
boundary = mimetools.choose_boundary()

# Request form
form_fields = []
form_fields.append(("code", code))
files = []
files.append(("imageFile", image.name, mimetype, image.read()))

# Request body
parts = []
part_boundary = "--%s" % boundary
parts.extend([part_boundary, 'Content-Disposition: form-data; name="%s"' % name, '', value,] for name, value in form_fields)
parts.extend([part_boundary, 'Content-Disposition: file; name="%s"; filename="%s"' % (field_name, filename), 'Content-Type: %s' % content_type, '', body, ] for field_name, filename, content_type, body in files)
flattened = list(itertools.chain(*parts))
flattened.append('--%s--' % boundary)
flattened.append('')
body = '\r\n'.join(flattened)

# Request and response
try:
    request = urllib2.Request(_url)
    request.add_header('Content-type', 'multipart/form-data; boundary=%s' % boundary)
    request.add_header('Content-length', len(body))
    request.add_data(body)
except Exception as e:
    print e.message

response = urllib2.urlopen(request)
if outputSong == "":
    print response.read()
else:
    saveFile(outputSong, response.read())
