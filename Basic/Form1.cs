using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Basic
{
    public partial class Basic : Form
    {

        CurrentUser currentUser;
        public static int RSA_KEY_LENGTH = 2048;
        public static int AES_KEY_LENGTH = 256;
        public static int NB_HASH_ROUNDS = 1000;

        String tmpPath = "";


        //simple()
        public Basic()
        {
            InitializeComponent();

            //Générer une paire de clés
            String pubKey, privKey;
            Crypt.GenRsaKeys(out pubKey, out privKey);

            currentUser = new CurrentUser(new Guid().ToString(), "fenx", pubKey, privKey);


            tmpPath = new Random().Next(1000000,9999999).ToString("X2") + ".tmp\\";

            if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

            List<String> listFiles = new List<String>();

            //BuildContainer(new List<User>(), listFiles, "test.bsc");
        }


        void BuildContainer(List<User> listUsers, List<String> listFiles, String archiveName)
        {
            String aesKey = Crypt.GenAesKey();
            Index idx = new Index(currentUser, aesKey);

            for (int i = 0; i < listUsers.Count; i++)
                idx.AddUser(listUsers[i]);

            WriteFile(tmpPath + "index.xml", idx.WriteToString());


            Tar t = new Tar();

            for (int i = 0; i < listFiles.Count; i++)
                t.AddFile(listFiles[i]);

            t.Archive(archiveName);

            Crypt.AESencryptFile(archiveName, Path.GetFileNameWithoutExtension(archiveName) + ".aes", aesKey);

            //t.ZipFile(archiveName);

        }


        String[] BrowseFolder(String directory, Boolean includeSubFolders = true)
        {
            List<String> listFiles = new List<String>();

            String[] files = Directory.GetFiles(directory);
            listFiles.AddRange(files);

            if (includeSubFolders)
            {
                String[] folders = Directory.GetDirectories(directory);

                for (int i = 0; i < folders.Length; i++)
                {
                    listFiles.AddRange(BrowseFolder(folders[i]));
                }
            }


            return listFiles.ToArray();
        }

        void OpenContainer(String archiveName)
        {
            Tar t = new Tar();

            if (Path.GetExtension(archiveName) == ".gz")
            {
                t.UnZipFile(archiveName);
                archiveName = Path.GetFileNameWithoutExtension(archiveName);
            }

            t.Extract(archiveName);

            Index idx = new Index(currentUser);
            List<User> listUsers = idx.ReadToListUser();

        }


        String HashRounds(String str)
        {
            for (int i = 0; i < NB_HASH_ROUNDS; i++) str = Crypt.SHA512(str);

            return str;
        }


        void Sdelete(String filename, int NbPasses = 1)
        {
            if (NbPasses < 1) NbPasses = 1;
            if (NbPasses > 10) NbPasses = 10;

            Process process = new Process();
            process.StartInfo.FileName = "sdelete.exe";
            process.StartInfo.Arguments = "-nobanner -p "+ NbPasses +" -r -s " + filename;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }


        void WriteFile(String filename, String data)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(data);
            }
        }

    }
}