namespace Kociemba
{
    // TODO: I should rewrie this library to make it clearer
    public class Tools
    {
        public static readonly string TableDirectory;

        private static readonly string AppdataDirectory;

        static Tools()
        {
            AppdataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CubeRobot");
            TableDirectory = Path.Combine(AppdataDirectory, "Tables");
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Check if the cube string s represents a solvable cube.
        // 0: Cube is solvable
        // -1: There is not exactly one facelet of each colour
        // -2: Not all 12 edges exist exactly once
        // -3: Flip error: One edge has to be flipped
        // -4: Not all corners exist exactly once
        // -5: Twist error: One corner has to be twisted
        // -6: Parity error: Two corners or two edges have to be exchanged
        /// <summary> Check if the cube definition string s represents a solvable cube. </summary>
        /// <param name="s"> is the cube definition string , see <seealso cref="Facelet"/> </param>
        /// <returns> 0: Cube is solvable<br>
        /// -1: There is not exactly one facelet of each colour<br>
        /// -2: Not all 12 edges exist exactly once<br>
        /// -3: Flip error: One edge has to be flipped<br>
        /// -4: Not all 8 corners exist exactly once<br>
        /// -5: Twist error: One corner has to be twisted<br>
        /// -6: Parity error: Two corners or two edges have to be exchanged </returns>
        public static int verify(string s)
        {
            int[] count = new int[6];
            try
            {
                for (int i = 0; i < 54; i++)
                {
                    count[(int)CubeColor.Parse(typeof(CubeColor), i.ToString())]++;
                }
            }
            catch (Exception)
            {
                return -1;
            }

            for (int i = 0; i < 6; i++)
            {
                if (count[i] != 9)
                {
                    return -1;
                }
            }

            FaceCube fc = new FaceCube(s);
            CubieCube cc = fc.toCubieCube();

            return cc.verify();
        }

        /// <summary>
        /// Generates a random cube.
        /// </summary>
        /// <returns>
        /// A random cube in the string representation. Each cube of the cube space has the same probability.
        /// </returns>
        public static string randomCube()
        {
            CubieCube cc = new CubieCube();
            Random gen = new Random();
            cc.setFlip((short)gen.Next(CoordCube.N_FLIP));
            cc.setTwist((short)gen.Next(CoordCube.N_TWIST));
            do
            {
                cc.setURFtoDLB(gen.Next(CoordCube.N_URFtoDLB));
                cc.setURtoBR(gen.Next(CoordCube.N_URtoBR));
            } while ((cc.edgeParity() ^ cc.cornerParity()) != 0);
            FaceCube fc = cc.toFaceCube();
            return fc.to_fc_String();
        }

        // https://stackoverflow.com/questions/7742519/c-sharp-export-write-multidimension-array-to-file-csv-or-whatever
        // Kristian Fenn: https://stackoverflow.com/users/989539/kristian-fenn
        public static void SerializeTable(string filename, short[,] array)
        {
            EnsureTableDirectory();

            using Stream s = File.Open(Path.Combine(TableDirectory, filename), FileMode.Create);
            using BinaryWriter writer = new(s);

            writer.Write(array.GetLength(0));
            writer.Write(array.GetLength(1));
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    writer.Write(array[i, j]);
                }
            }
        }

        public static short[,] DeserializeTable(string filename)
        {
            EnsureTableDirectory();

            using Stream s = File.Open(Path.Combine(TableDirectory, filename), FileMode.Open);
            using BinaryReader reader = new(s);

            int rowCount = reader.ReadInt32();
            int colCount = reader.ReadInt32();

            short[,] array = new short[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    array[i, j] = reader.ReadInt16();
                }
            }

            return array;
        }

        public static void SerializeSbyteArray(string filename, sbyte[] array)
        {
            EnsureTableDirectory();

            using Stream s = File.Open(Path.Combine(TableDirectory, filename), FileMode.Create);
            using BinaryWriter writer = new(s);

            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }
        }

        public static sbyte[] DeserializeSbyteArray(string filename)
        {
            EnsureTableDirectory();

            using Stream s = File.Open(Path.Combine(TableDirectory, filename), FileMode.Open);
            using BinaryReader reader = new(s);

            int length = reader.ReadInt32();

            sbyte[] array = new sbyte[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadSByte();
            }

            return array;
        }

        // https://stackoverflow.com/questions/3695163/filestream-and-creating-folders
        // Joe: https://stackoverflow.com/users/13087/joe

        public static void EnsureTableDirectory()
        {
            if (!Directory.Exists(AppdataDirectory))
                Directory.CreateDirectory(AppdataDirectory);

            if (!Directory.Exists(TableDirectory))
                Directory.CreateDirectory(TableDirectory);
        }
    }
}