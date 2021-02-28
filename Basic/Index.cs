using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Basic
{
    class Index
    {
        List<User> listUsers;
        String aesKey;

        public Index(User sender, String aesKey)
        {
            listUsers = new List<User>();
            listUsers.Add(sender);

            this.aesKey = aesKey;
        }

        public Index(User sender)
        {
            listUsers = new List<User>();
            listUsers.Add(sender);
        }

        public void AddUser(User userToAdd)
        {
            listUsers.Add(userToAdd);
        }

        public Boolean RemoveUser(User userToRemove)
        {
            int idx = listUsers.IndexOf(userToRemove);
            
            if (idx > 0) //do not remove sender
                return listUsers.Remove(userToRemove);

            return false;
        }

        public String WriteToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<users>");

            for (int i = 0; i < listUsers.Count; i++)
            {
                sb.AppendLine("<user>");
                sb.AppendLine("<GUID>" + listUsers[i].GUID + "</GUID>" +
                                "<name>" + listUsers[i].Name + "</name>" +
                                "<key>" + Crypt.RsaEncrypt(aesKey, listUsers[i].PubKey) + "</key>");
                sb.AppendLine("</user>");
            }

            sb.AppendLine("</users>");

            return sb.ToString();
        }


        public List<User> ReadToListUser()
        {
            XmlDocument doc = new XmlDocument();
            using (FileStream fs = new FileStream("index.xml", FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
            }

            XmlNodeList xmlUsers = doc.GetElementsByTagName("users")[0].ChildNodes;

            foreach (XmlNode user in xmlUsers)
            {
                listUsers.Add(new User(guid: user.ChildNodes[0].InnerText,
                                        name: user.ChildNodes[1].InnerText,
                                        pubKey: user.ChildNodes[2].InnerText));
            }

            return listUsers;
        }
    }
}
