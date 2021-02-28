using System;
using System.Collections.Generic;
using tar_cs;
using System.IO;
using System.IO.Compression;


namespace Basic
{
    class Tar
    {
        List<String> files;

        /// <summary>
        /// Create new tar archive
        /// </summary>
        /// <param name="archiveName"></param>
        public Tar()
        {
            this.files = new List<String>();
        }

        public void AddFile(String fileToAdd)
        {
            files.Add(fileToAdd);
        }

        public Boolean RemoveFile(String fileToRemove)
        {
            return files.Remove(fileToRemove);
        }



        public List<String> BrowseFolder(String folderName, Boolean recursive = true)
        {
            List<String> listFiles = new List<String>();

            String[] files = Directory.GetFiles(folderName);
            listFiles.AddRange(files);

            if (recursive)
            {
                String[] folders = Directory.GetDirectories(folderName);

                for (int i = 0; i < folders.Length; i++)
                {
                    listFiles.AddRange(BrowseFolder(folders[i]));
                }
            }

            return listFiles;
        }


        public void Archive(String archiveName)
        {
            using (FileStream fsTar = new FileStream(archiveName, FileMode.CreateNew))
            {
                using (TarWriter tar = new TarWriter(fsTar))
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        tar.Write(files[i]);
                    }
                }
            }
        }


        public Byte[] ArchiveToMemory(String archiveName)
        {
            MemoryStream msTar = new MemoryStream();
            using (TarWriter tar = new TarWriter(msTar))
            {
                for (int i = 0; i < files.Count; i++)
                {
                    tar.Write(files[i]);
                }
            }

            return msTar.ToArray();
        }


        public void Extract(String archiveName, String destinaton = ".")
        {
            using (FileStream fsTar = File.OpenRead(archiveName))
            {
                TarReader reader = new TarReader(fsTar);
                reader.ReadToEnd(destinaton);
            }
        }


        public void ZipFile(String fileToZip)
        {
            String zippedFile = Path.GetFileNameWithoutExtension(fileToZip) + ".gz";

            using (FileStream fsIn = new FileStream(fileToZip, FileMode.Open))
            using (FileStream fsOut = new FileStream(zippedFile, FileMode.CreateNew))
            using (GZipStream zip = new GZipStream(fsOut, CompressionMode.Compress))
            {
                Byte[] buffer = new Byte[1024];
                int n;

                while ((n = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                    zip.Write(buffer, 0, n);
            }
        }


        public void UnZipFile(String fileToUnZip)
        {
            String unzippedFile = Path.GetFileNameWithoutExtension(fileToUnZip);

            using (FileStream fsIn = new FileStream(fileToUnZip, FileMode.Open))
            using (FileStream fsOut = new FileStream(unzippedFile, FileMode.CreateNew))
            using (GZipStream zip = new GZipStream(fsIn, CompressionMode.Decompress))
            {
                Byte[] buffer = new Byte[1024];
                int n;

                while ((n = zip.Read(buffer, 0, buffer.Length)) > 0)
                    fsOut.Write(buffer, 0, n);
            }
        }

    }
}
