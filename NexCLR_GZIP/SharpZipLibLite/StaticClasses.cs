using ICSharpCode.SharpZipLib.Zip.Compression;
using System;

namespace ICSharpCode.SharpZipLib
{
    /*
     * Created By: bradjoss@hotmail.com
     * On: Nov 3, 2011
     * 
     * WARNING: This version of SharpZipLib (in the /SharpZipLibLite/ folder) is significanly
     * modified from the standard version.  I has had all parts not necessary for stream compression
     * removed (i.e. all the file management stuff), and as this file attemps to accomplish, is
     * reasonably thread safe.  It has been tested with running 10000 parallel calls on about 20
     * differnet threads, without failure, and general code analysis looks as though it should be
     * thread safe, knock on wood.
    */

    /// <summary>
    /// Class to hold variables that are listed as public static in the standard
    /// SharpZipLib.  The SQL CLR does not allow this on "Safe" assemblies, and
    /// this the workaound to the problem.
    /// </summary>
    internal class StaticClasses
    {
        /// <summary>
        /// Container for public static variables used by DeflaterHuffman
        /// </summary>
        public DeflaterHuffmanStatic DeflaterHuffmanStaticInstance;
        /// <summary>
        /// Container for public static variables used by InflaterHuffmanTree
        /// </summary>
        public InflaterHuffmanTreeStatic InflaterHuffmanTreeStaticInstance;

        /// <summary>
        /// Init the static classes to their default values
        /// </summary>
        public StaticClasses()
        {
            DeflaterHuffmanStaticInstance = new DeflaterHuffmanStatic();
            InflaterHuffmanTreeStaticInstance = new InflaterHuffmanTreeStatic();
        }

        /// <summary>
        /// Container for public static classes used by DeflaterHuffman
        /// </summary>
        public class DeflaterHuffmanStatic
        {
            public short[] staticLCodes;
            public byte[] staticLLength;
            public short[] staticDCodes;
            public byte[] staticDLength;

            // Code copy and pasted from "static DeflaterHuffmanStatic()"
            public DeflaterHuffmanStatic() 
            {
                // See RFC 1951 3.2.6
                // Literal codes
                staticLCodes = new short[DeflaterHuffman.LITERAL_NUM];
                staticLLength = new byte[DeflaterHuffman.LITERAL_NUM];

                int i = 0;
                while (i < 144) {
                    staticLCodes[i] = DeflaterHuffman.BitReverse((0x030 + i) << 8);
                    staticLLength[i++] = 8;
                }

                while (i < 256) {
                    staticLCodes[i] = DeflaterHuffman.BitReverse((0x190 - 144 + i) << 7);
                    staticLLength[i++] = 9;
                }

                while (i < 280) {
                    staticLCodes[i] = DeflaterHuffman.BitReverse((0x000 - 256 + i) << 9);
                    staticLLength[i++] = 7;
                }

                while (i < DeflaterHuffman.LITERAL_NUM)
                {
                    staticLCodes[i] = DeflaterHuffman.BitReverse((0x0c0 - 280 + i) << 8);
                    staticLLength[i++] = 8;
                }
            
                // Distance codes
                staticDCodes = new short[DeflaterHuffman.DIST_NUM];
                staticDLength = new byte[DeflaterHuffman.DIST_NUM];

                for (i = 0; i < DeflaterHuffman.DIST_NUM; i++)
                {
                    staticDCodes[i] = DeflaterHuffman.BitReverse(i << 11);
                    staticDLength[i] = 5;
                }
            }
        }

        /// <summary>
        /// Container for public static classes used by InflaterHuffman
        /// </summary>
        public class InflaterHuffmanTreeStatic
        {
            /// <summary>
            /// Literal length tree
            /// </summary>
            public InflaterHuffmanTree defLitLenTree;

            /// <summary>
            /// Distance tree
            /// </summary>
            public InflaterHuffmanTree defDistTree;

            // Code copy and pasted from "static InflaterHuffmanTree()"
            public InflaterHuffmanTreeStatic()
            {
                try {
                    byte[] codeLengths = new byte[288];
                    int i = 0;

                    while (i < 144) {
                        codeLengths[i++] = 8;
                    }
                    while (i < 256) {
                        codeLengths[i++] = 9;
                    }
                    while (i < 280) {
                        codeLengths[i++] = 7;
                    }
                    while (i < 288) {
                        codeLengths[i++] = 8;
                    }

                    defLitLenTree = new InflaterHuffmanTree(codeLengths);                
                    codeLengths = new byte[32];
                    i = 0;

                    while (i < 32) {
                        codeLengths[i++] = 5;
                    }

                    defDistTree = new InflaterHuffmanTree(codeLengths);
                } catch (Exception) {
                    throw new SharpZipBaseException("InflaterHuffmanTree: static tree length illegal");
                }
            }
        }
    }
}
