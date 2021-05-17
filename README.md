# StreamerDemo
FLIR sample project .NET windows forms

Some changes were made:
1) Added 3 buttons for capturing images.
2) Some code was added for capturing images.
3) Some changes with streaming window size.

The main idea for capturing images is to send to camera signals for Digital Input, which are "1" and "0".
Triggers on for Digital I/O (Trigg on: 1).
Tested on FLIR AX8 thermal camera.

There is 2 files missed in this repository: 
1) ImageProcessingWrapper.dll
2) avcodec-56.dll
You can download them from official Flir website (link below).


Original sampel code were taken from official Flir website: https://support.flir.com/SwDownload/Assets/Atlas/dotnet/flir-atlas-samples.zip
Was used for educational purposes.
Copyright: FLIR Systems Inc
