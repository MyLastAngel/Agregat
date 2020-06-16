using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ArgDb;

namespace AgrStorage1
{
    //public class AgrStorage
    //{
    //    public string AgrPath
    //    {
    //        get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Agr"; }
    //    }
    //    public bool LoadProjectList(List<int> projectsId)
    //    {
    //        try
    //        {
    //            //string[] files = Directory.GetFiles(AgrPath + "\\Projects", "*.xml");
    //            //foreach (string file in files)
    //            //{
    //            //    try
    //            //    {
    //            //        projectsId.Add(int.Parse(file));
    //            //    }
    //            //    catch (Exception)
    //            //    {
    //            //    }
    //            //}
    //            string[] dirs = Directory.GetFiles(AgrPath + "\\Projects");
    //            foreach (string dir in dirs)
    //            {
    //                try
    //                {
    //                    projectsId.Add(int.Parse(dir));
    //                }
    //                catch (Exception)
    //                {
    //                }
    //            }
    //            return true;
    //        }
    //        catch (DirectoryNotFoundException exc)
    //        {
    //            return false;
    //        }
    //    }
    //    public bool LoadProject(AgrDataSet ds, int id)
    //    {
    //        string file = AgrPath + "\\Projects\\" + id + "\\info.xml";
    //        try
    //        {
    //            XDocument projectXml = XDocument.Load(file);
    //            if (projectXml.Root != null)
    //            {
    //                foreach (XElement e in projectXml.Root.Elements())
    //                {
    //                    try
    //                    {
    //                        XAttribute attrClient = e.Attribute("Client");
    //                        if (attrClient == null)
    //                            continue;
    //                        int clientId = int.Parse(attrClient.Value);
    //                        AgrDataSet.ClientRow drClient = ds.Client.FindById(clientId);

    //                        XAttribute attrProduct = e.Attribute("Product");
    //                        if (attrProduct == null)
    //                            continue;
    //                        int productId = int.Parse(attrProduct.Value);
    //                        AgrDataSet.ProductRow drProduct = ds.Product.FindById(productId);

    //                        XAttribute attrDate = e.Attribute("Date");
    //                        DateTime date = attrDate == null ? DateTime.Now : DateTime.Parse(attrDate.Value);

    //                        AgrDataSet.ProjectsRow drProjects = ds.Projects.FindById(id);

    //                        var d = DateTime.MinValue;
    //                        ds.Project.AddProjectRow(drProjects, "", date, "",
    //                            d, d, d, "", d, d, d, d, d, d, d, d, d,
    //                            "", "", "", "", "", "", false,
    //                            d, d, d, d, d, d);
    //                    }
    //                    catch (Exception)
    //                    {
    //                    }
    //                }
    //            }
    //            return true;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }
    //    public bool LoadOptionsList(AgrDataSet ds)
    //    {
    //        string file = AgrPath + "\\Options.xml";
    //        try
    //        {
    //            XDocument projectXml = XDocument.Load(file);
    //            if (projectXml.Root != null)
    //            {
    //                foreach (XElement e in projectXml.Root.Elements())
    //                {
    //                    try
    //                    {
    //                        XAttribute attr = e.Attribute("Name");
    //                        if (attr == null)
    //                            continue;
    //                        string name = attr.Value;

    //                        attr = e.Attribute("Description");
    //                        if (attr == null)
    //                            continue;
    //                        string description = attr.Value;

    //                        ds.Option.AddOptionRow(name, description);
    //                    }
    //                    catch (Exception)
    //                    {
    //                    }
    //                }
    //            }
    //            return true;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }
    //    public bool LoadProductList(AgrDataSet ds)
    //    {
    //        string file = AgrPath + "\\Products.xml";
    //        try
    //        {
    //            XDocument projectXml = XDocument.Load(file);
    //            if (projectXml.Root != null)
    //            {
    //                foreach (XElement e in projectXml.Root.Elements())
    //                {
    //                    try
    //                    {
    //                        XAttribute attrId = e.Attribute("Id");
    //                        if (attrId == null)
    //                            continue;
    //                        int id = int.Parse(attrId.Value);

    //                        XAttribute attr = e.Attribute("Name");
    //                        string name = attr == null ? "" : attr.Value;

    //                        attr = e.Attribute("Description");
    //                        string description = attr == null ? "" : attr.Value;

    //                        ds.Product.AddProductRow(id, name, description);
    //                    }
    //                    catch (Exception)
    //                    {
    //                    }
    //                }
    //            }
    //            return true;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }
    //    public bool LoadClientList(AgrDataSet ds)
    //    {
    //        string file = AgrPath + "\\Clients.xml";
    //        try
    //        {
    //            XDocument projectXml = XDocument.Load(file);
    //            if (projectXml.Root != null)
    //            {
    //                foreach (XElement e in projectXml.Root.Elements())
    //                {
    //                    try
    //                    {
    //                        XAttribute attrId = e.Attribute("Id");
    //                        if (attrId == null)
    //                            continue;
    //                        int id = int.Parse(attrId.Value);
                        
    //                        XAttribute attr = e.Attribute("Name");
    //                        string name = attr == null ? "" : attr.Value;

    //                        attr = e.Attribute("Description");
    //                        string description = attr == null ? "" : attr.Value;

    //                        ds.Client.AddClientRow(id, name, description);
    //                    }
    //                    catch (Exception)
    //                    {
    //                    }
    //                }
    //            }
    //            return true;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }

    //    public bool LoadUsers(AgrDataSet ds)
    //    {
    //        string file = AgrPath + "\\Users.xml";
    //        try
    //        {
    //            XDocument projectXml = XDocument.Load(file);
    //            if (projectXml.Root != null)
    //            {
    //                foreach (XElement e in projectXml.Root.Elements("Group"))
    //                {
    //                    try
    //                    {
    //                        XAttribute attrGroup = e.Attribute("Name");
    //                        if (attrGroup == null)
    //                            continue;
    //                        string group = attrGroup.Value;
    //                        //AgrDataSet.GroupRow drGroup = ds.Group.AddGroupRow(group);
    //                        //foreach (XElement e1 in e.Elements("User"))
    //                        //{
    //                        //    XAttribute attrUser = e1.Attribute("Name");
    //                        //    if (attrUser == null)
    //                        //        continue;
    //                        //    string user = attrUser.Value;
    //                        //    ds.User.AddUserRow(drGroup, user);
    //                        //}
    //                    }
    //                    catch (Exception)
    //                    {
    //                    }
    //                }
    //            }
    //            return true;
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }
    //}
}
