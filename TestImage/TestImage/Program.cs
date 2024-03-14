
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ClassLibrary1;

// See https://aka.ms/new-console-template for more information

var qrDecoder = new QRDecoder();
QRCodeTrace.Open("QRCodeDecoderTrace.txt");
QRCodeTrace.Write("QRCodeDecoder");
Console.WriteLine("Hello, World!");
var curDir = string.Concat(Directory.GetCurrentDirectory(), @"\DoubleQRCode.png");
QRCodeResult[] QRCodeResultArray = qrDecoder.ImageDecoder(curDir);

QRCodeTrace.Format("****");
QRCodeTrace.Format("Decode image: {0} ", curDir);
QRCodeTrace.Format("Image width: {0}, Height: {1}", 25,25);

Console.WriteLine(QRCodeResultArray[0].QRCodeDimension.ToString());
Console.WriteLine(QRCodeResultArray[0].ErrorCorrection.ToString());

// display ECI value
Console.WriteLine(QRCodeResultArray[0].ECIAssignValue >= 0 ? QRCodeResultArray[0].ECIAssignValue.ToString() : null);

// convert results to text
Console.WriteLine(ConvertResultToDisplayString(QRCodeResultArray));

 static string ConvertResultToDisplayString
(
    QRCodeResult[] DataByteArray
)
{
    // no QR code
    if (DataByteArray == null) return string.Empty;

    // image has one QR code
    if (DataByteArray.Length == 1) return SingleQRCodeResult(QRDecoder.ByteArrayToStr(DataByteArray[0].DataArray));

    // image has more than one QR code
    StringBuilder Str = new();
    for (int Index = 0; Index < DataByteArray.Length; Index++)
    {
        if (Index != 0) Str.Append("\r\n");
        Str.AppendFormat("QR Code {0}\r\n", Index + 1);
        Str.Append(SingleQRCodeResult(QRDecoder.ByteArrayToStr(DataByteArray[Index].DataArray)));
    }
    return Str.ToString();
}
 static string SingleQRCodeResult
(
    string Result
)
{
    int Index;
    for (Index = 0; Index < Result.Length && (Result[Index] >= ' ' && Result[Index] <= '~' || Result[Index] >= 160); Index++) ;
    if (Index == Result.Length) return Result;

    StringBuilder Display = new(Result[..Index]);
    for (; Index < Result.Length; Index++)
    {
        char OneChar = Result[Index];
        if (OneChar >= ' ' && OneChar <= '~' || OneChar >= 160)
        {
            Display.Append(OneChar);
            continue;
        }

        if (OneChar == '\r')
        {
            Display.Append("\r\n");
            if (Index + 1 < Result.Length && Result[Index + 1] == '\n') Index++;
            continue;
        }

        if (OneChar == '\n')
        {
            Display.Append("\r\n");
            continue;
        }

        Display.Append('¿');
    }
    return Display.ToString();
}