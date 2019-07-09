using System;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Microsoft.SqlServer.Server;

public class Gzip
{
    #region Const
    /// <summary>
    /// Encoding used when working with Text
    /// </summary>
    private static readonly Encoding SERIALIZATION_ENCODING = Encoding.UTF8;

    /// <summary>
    /// Default buffer size
    /// </summary>
    private const int BUFFER_SIZE = (16 * 1024);
    #endregion

    [SqlFunction]
    public static SqlChars CompressString(SqlChars input)
    {
        if (input.IsNull || input == SqlChars.Null || input.Value == null)
        {
            return SqlChars.Null;
        }

        return new SqlChars(Convert.ToBase64String(ZipBytes(Encoding.UTF8.GetBytes(input.Value))));
    }

    [SqlFunction]
    public static SqlChars DecompressString(SqlChars input)
    {
        if (input.IsNull || input == SqlChars.Null || input.Value == null)
        {
            return SqlChars.Null;
        }

        return new SqlChars(UnzipBytes(Convert.FromBase64CharArray(input.Value, 0, input.Value.Length)));
    }

    private static byte[] ZipBytes(byte[] buffer)
    {
        int BufferSize = buffer.Length;

        using (var memoryStream = new MemoryStream())
        {
            var sc = new StaticClasses();

            using (var stream = new DeflaterOutputStream(memoryStream, ref sc))
            {
                stream.Write(buffer, 0, BufferSize);
            }

            return memoryStream.ToArray();
        }
    }

    private static string UnzipBytes(byte[] byteInput)
    {
        var uncompressedString = new StringBuilder();
        byte[] writeData = new byte[BUFFER_SIZE];
        var sc = new StaticClasses();

        using (Stream s2 = new InflaterInputStream(new MemoryStream(byteInput), ref sc))
        {
            while (true)
            {
                int size = s2.Read(writeData, 0, writeData.Length);

                if (size > 0)
                {
                    uncompressedString.Append(SERIALIZATION_ENCODING.GetString(writeData, 0, size));
                }
                else
                {
                    break;
                }
            }

            s2.Close();
        }

        return uncompressedString.ToString();
    }
}
