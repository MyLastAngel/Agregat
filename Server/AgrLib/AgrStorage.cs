using ArgLib.Logger;
using System.IO;
using System.Xml.Linq;

namespace ArgDb
{
    public class AgrStorage
    {
        #region Поля
        const string unit = "AgrStorage";
        #endregion

        public static string AgrPath
        {
            get { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\Agr"; }
        }

        public bool LoadProjectList(AgrDataSet ds)
        {
            try
            {
                //    string[] files = Directory.GetFiles(AgrPath + "\\Projects", "*.xml");
                //    foreach (string file in files)
                //    {
                //        try
                //        {
                //            ds.Projects.AddProjectsRow(int.Parse(Path.GetFileNameWithoutExtension(file)));
                //        }
                //        catch (System.Exception)
                //        {
                //        }
                //    }
                var rootDir = AgrPath + "\\Projects";
                if (!System.IO.Directory.Exists(rootDir))
                    System.IO.Directory.CreateDirectory(rootDir);
                LogManager.LogInfo(unit, string.Format("(LoadProjectList) Загрузка списка проектов из каталога: {0}", rootDir));

                string[] dirs = System.IO.Directory.GetDirectories(rootDir);
                foreach (string dir in dirs)
                {
                    try
                    {
                        int id = int.Parse(System.IO.Path.GetFileNameWithoutExtension(dir));
                        string[] lines = System.IO.File.ReadAllLines(dir + "\\date.txt");
                        System.DateTime date = System.DateTime.Parse(lines[0]);
                        var endDate = System.DateTime.MinValue;
                        if (lines.Length > 1)
                        {
                            try
                            {
                                endDate = System.DateTime.Parse(lines[1]);
                            }
                            catch
                            {
                            }
                        }

                        ds.Projects.AddProjectsRow(id, date, endDate);
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.LogError(unit, "(LoadProjectList) Ошибка загрузки: " + dir, ex);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, "(LoadProjectList) Ошибка загрузки Projects", ex);
                return false;
            }
        }

        public bool RemoveProject(int id)
        {
            string dir = AgrPath + "\\Projects\\" + id;
            LogManager.LogInfo(unit, string.Format("(RemoveProject) Удаление проекта {0} каталог: {1}", id, dir));

            try
            {
                System.IO.Directory.Delete(dir, true);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, "(RemoveProject) Ошибка удаления: " + dir, ex);
                return false;
            }
        }

        public bool LoadProject(AgrDataSet ds, int id)
        {
            string file = AgrPath + "\\Projects\\" + id + "\\info.xml";
            LogManager.LogInfo(unit, string.Format("(LoadProject) Загрузка проекта {0} файл: {1}", id, file));
            try
            {
                XDocument projectXml = XDocument.Load(file);
                if (projectXml.Root != null)
                {
                    try
                    {
                        XAttribute attr = projectXml.Root.Attribute("Client");
                        if (attr == null)
                        {
                            return false;
                        }
                        int clientId = int.Parse(attr.Value);
                        AgrDataSet.ClientRow drClient = ds.Client.FindById(clientId);

                        attr = projectXml.Root.Attribute("Product");
                        if (attr == null)
                        {
                            return false;
                        }
                        int productId = int.Parse(attr.Value);
                        AgrDataSet.ProductRow drProduct = ds.Product.FindById(productId);

                        AgrDataSet.ProjectsRow drProjects = ds.Projects.FindById(id);

                        attr = projectXml.Root.Attribute("Name");
                        string name = attr == null ? "" : attr.Value;

                        attr = projectXml.Root.Attribute("IsManagerSetPlanDate");
                        bool IsManagerSetPlanDate = attr != null && bool.Parse(attr.Value);

                        attr = projectXml.Root.Attribute("CustomerName");
                        string customerName = attr == null ? "" : attr.Value;

                        //attr = projectXml.Root.Attribute("Date");
                        //System.DateTime date = attr == null ? System.DateTime.Now : System.DateTime.Parse(attr.Value);

                        //XElement e = projectXml.Root.Element("Comments");
                        //string comments = e == null ? "" : e.Value;

                        XElement e = projectXml.Root.Element("ITO");
                        System.DateTime Time_ITO_G_Plan;
                        System.DateTime Time_ITO_E_Plan;
                        System.DateTime Time_ITO_R_Plan;
                        System.DateTime Time_ITO_G_Actual;
                        System.DateTime Time_ITO_E_Actual;
                        System.DateTime Time_ITO_R_Actual;
                        bool Is_ITO_G_NotNeed;
                        bool Is_ITO_E_NotNeed;
                        bool Is_ITO_R_NotNeed;
                        string ITO_R_Mode;
                        if (e != null)
                        {
                            attr = e.Attribute("Time_ITO_G_Plan");
                            Time_ITO_G_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_ITO_E_Plan");
                            Time_ITO_E_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_ITO_R_Plan");
                            Time_ITO_R_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("Time_ITO_G_Actual");
                            Time_ITO_G_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_ITO_E_Actual");
                            Time_ITO_E_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_ITO_R_Actual");
                            Time_ITO_R_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("ITO_R_Mode");
                            ITO_R_Mode = attr == null ? "" : attr.Value;

                            attr = e.Attribute("Is_ITO_G_NotNeed");
                            Is_ITO_G_NotNeed = attr != null && bool.Parse(attr.Value);
                            attr = e.Attribute("Is_ITO_E_NotNeed");
                            Is_ITO_E_NotNeed = attr != null && bool.Parse(attr.Value);
                            attr = e.Attribute("Is_ITO_R_NotNeed");
                            Is_ITO_R_NotNeed = attr != null && bool.Parse(attr.Value);
                        }
                        else
                        {
                            Time_ITO_G_Plan = System.DateTime.MinValue;
                            Time_ITO_E_Plan = System.DateTime.MinValue;
                            Time_ITO_R_Plan = System.DateTime.MinValue;

                            Time_ITO_G_Actual = System.DateTime.MinValue;
                            Time_ITO_E_Actual = System.DateTime.MinValue;
                            Time_ITO_R_Actual = System.DateTime.MinValue;

                            Is_ITO_G_NotNeed = false;
                            Is_ITO_E_NotNeed = false;
                            Is_ITO_R_NotNeed = false;
                            ITO_R_Mode = "";
                        }

                        e = projectXml.Root.Element("WH");
                        System.DateTime Time_WH_G_Plan;
                        System.DateTime Time_WH_E_Plan;
                        System.DateTime Time_WH_R_Plan;

                        System.DateTime Time_WH_G_Actual;
                        System.DateTime Time_WH_E_Actual;
                        System.DateTime Time_WH_R_Actual;

                        bool Is_WH_G_NotNeed;
                        bool Is_WH_E_NotNeed;
                        bool Is_WH_R_NotNeed;

                        System.DateTime Time_WH_E_Requests_Create;
                        System.DateTime Time_WH_G_Requests_Create;
                        if (e != null)
                        {
                            attr = e.Attribute("Time_WH_G_Plan");
                            Time_WH_G_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_WH_E_Plan");
                            Time_WH_E_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_WH_R_Plan");
                            Time_WH_R_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("Time_WH_G_Actual");
                            Time_WH_G_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_WH_E_Actual");
                            Time_WH_E_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_WH_R_Actual");
                            Time_WH_R_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("Is_WH_G_NotNeed");
                            Is_WH_G_NotNeed = attr != null && bool.Parse(attr.Value);
                            attr = e.Attribute("Is_WH_E_NotNeed");
                            Is_WH_E_NotNeed = attr != null && bool.Parse(attr.Value);
                            attr = e.Attribute("Is_WH_R_NotNeed");
                            Is_WH_R_NotNeed = attr != null && bool.Parse(attr.Value);

                            attr = e.Attribute("Time_WH_E_Requests_Create");
                            Time_WH_E_Requests_Create = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_WH_G_Requests_Create");
                            Time_WH_G_Requests_Create = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                        }
                        else
                        {
                            Time_WH_G_Plan = System.DateTime.MinValue;
                            Time_WH_E_Plan = System.DateTime.MinValue;
                            Time_WH_R_Plan = System.DateTime.MinValue;

                            Time_WH_G_Actual = System.DateTime.MinValue;
                            Time_WH_E_Actual = System.DateTime.MinValue;
                            Time_WH_R_Actual = System.DateTime.MinValue;

                            Is_WH_G_NotNeed = false;
                            Is_WH_E_NotNeed = false;
                            Is_WH_R_NotNeed = false;

                            Time_WH_E_Requests_Create = System.DateTime.MinValue;
                            Time_WH_G_Requests_Create = System.DateTime.MinValue;
                        }

                        e = projectXml.Root.Element("OMTS");
                        System.DateTime Time_OMTS_G_Plan;
                        System.DateTime Time_OMTS_E_Plan;

                        System.DateTime Time_OMTS_G_Actual;
                        System.DateTime Time_OMTS_E_Actual;
                        if (e != null)
                        {
                            attr = e.Attribute("Time_OMTS_G_Plan");
                            Time_OMTS_G_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_OMTS_E_Plan");
                            Time_OMTS_E_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("Time_OMTS_G_Actual");
                            Time_OMTS_G_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_OMTS_E_Actual");
                            Time_OMTS_E_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                        }
                        else
                        {
                            Time_OMTS_G_Plan = System.DateTime.MinValue;
                            Time_OMTS_E_Plan = System.DateTime.MinValue;

                            Time_OMTS_G_Actual = System.DateTime.MinValue;
                            Time_OMTS_E_Actual = System.DateTime.MinValue;
                        }

                        e = projectXml.Root.Element("Time");
                        System.DateTime TimeBegin;
                        System.DateTime TimeEndPlaned;
                        System.DateTime Com_New_Time;
                        if (e != null)
                        {
                            attr = e.Attribute("TimeBegin");
                            TimeBegin = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("TimeEndPlaned");
                            TimeEndPlaned = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Com_New_Time");
                            Com_New_Time = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                        }
                        else
                        {
                            TimeBegin = System.DateTime.MinValue;
                            TimeEndPlaned = System.DateTime.MinValue;
                            Com_New_Time = System.DateTime.MinValue;
                        }

                        e = projectXml.Root.Element("MF");
                        string MF_Level;
                        string MF_Rama;
                        string MF_Post;
                        string MF_Agregat;
                        string MF_SH_Place;
                        string MF_SH;
                        bool MF_Test;
                        System.DateTime MF_Time_Plan;
                        System.DateTime MF_Time_Test_Plan;
                        System.DateTime MF_Time_Test_Actual;
                        System.DateTime MF_Time;
                        string MF_Collector;
                        string MF_Complete_Percentage;
                        if (e != null)
                        {
                            attr = e.Attribute("MF_Level");
                            MF_Level = attr == null ? "" : attr.Value;
                            attr = e.Attribute("MF_Rama");
                            MF_Rama = attr == null ? "" : attr.Value;
                            attr = e.Attribute("MF_Post");
                            MF_Post = attr == null ? "" : attr.Value;
                            attr = e.Attribute("MF_Agregat");
                            MF_Agregat = attr == null ? "" : attr.Value;
                            attr = e.Attribute("MF_SH_Place");
                            MF_SH_Place = attr == null ? "" : attr.Value;
                            attr = e.Attribute("MF_SH");
                            MF_SH = attr == null ? "" : attr.Value;

                            attr = e.Attribute("MF_Test");
                            MF_Test = attr != null && bool.Parse(attr.Value);

                            attr = e.Attribute("MF_Time_Plan");
                            MF_Time_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("MF_Time_Test_Plan");
                            MF_Time_Test_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("MF_Time_Test_Actual");
                            MF_Time_Test_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("MF_Time");
                            MF_Time = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("MF_Collector");
                            MF_Collector = attr == null ? "" : attr.Value;
                            attr = e.Attribute("MF_Complete_Percentage");
                            MF_Complete_Percentage = attr == null ? "" : attr.Value;
                        }
                        else
                        {
                            MF_Level = "";
                            MF_Rama = "";
                            MF_Post = "";
                            MF_Agregat = "";
                            MF_SH_Place = "";
                            MF_SH = "";
                            MF_Test = false;
                            MF_Time_Plan = System.DateTime.MinValue;
                            MF_Time_Test_Plan = System.DateTime.MinValue;
                            MF_Time_Test_Actual = System.DateTime.MinValue;
                            MF_Time = System.DateTime.MinValue;
                            MF_Collector = "";
                            MF_Complete_Percentage = "";
                        }

                        e = projectXml.Root.Element("OTK");
                        System.DateTime Time_OTK_Plan;
                        System.DateTime Time_OTK_G_Actual;
                        System.DateTime Time_OTK_E_Actual;
                        bool Is_OTK_G_NotNeed;
                        bool Is_OTK_E_NotNeed;
                        if (e != null)
                        {
                            attr = e.Attribute("Time_OTK_Plan");
                            Time_OTK_Plan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_OTK_G_Actual");
                            Time_OTK_G_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);
                            attr = e.Attribute("Time_OTK_E_Actual");
                            Time_OTK_E_Actual = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("Is_OTK_G_NotNeed");
                            Is_OTK_G_NotNeed = attr != null && bool.Parse(attr.Value);
                            attr = e.Attribute("Is_OTK_E_NotNeed");
                            Is_OTK_E_NotNeed = attr != null && bool.Parse(attr.Value);
                        }
                        else
                        {
                            Time_OTK_Plan = System.DateTime.MinValue;
                            Time_OTK_G_Actual = System.DateTime.MinValue;
                            Time_OTK_E_Actual = System.DateTime.MinValue;
                            Is_OTK_G_NotNeed = false;
                            Is_OTK_E_NotNeed = false;
                        }

                        e = projectXml.Root.Element("Options");
                        string options = e == null ? "" : e.Value;
                        e = projectXml.Root.Element("COM_Package_Type");
                        string COM_Package_Type = e == null ? "" : e.Value;
                        e = projectXml.Root.Element("IsStop");
                        bool IsStop = e == null ? false : bool.Parse(e.Value);


                        ds.Project.AddProjectRow(drProjects,
                            name, customerName, IsManagerSetPlanDate,
                            Time_ITO_G_Plan, Time_ITO_G_Actual, Time_ITO_E_Plan, Time_ITO_E_Actual, Time_ITO_R_Plan, Time_ITO_R_Actual, ITO_R_Mode, Is_ITO_G_NotNeed, Is_ITO_E_NotNeed, Is_ITO_R_NotNeed,
                            Time_WH_G_Plan, Time_WH_G_Actual, Time_WH_E_Plan, Time_WH_E_Actual, Time_WH_R_Plan, Time_WH_R_Actual, Is_WH_G_NotNeed, Is_WH_E_NotNeed, Is_WH_R_NotNeed, Time_WH_E_Requests_Create, Time_WH_G_Requests_Create,
                            Time_OMTS_G_Plan, Time_OMTS_G_Actual, Time_OMTS_E_Plan, Time_OMTS_E_Actual,
                            TimeBegin, TimeEndPlaned, Com_New_Time,
                            MF_Level, MF_Rama, MF_Post, MF_Agregat, MF_SH_Place, MF_SH, MF_Test, MF_Time_Plan, MF_Time_Test_Plan, MF_Time_Test_Actual, MF_Time, MF_Collector, MF_Complete_Percentage,
                            Time_OTK_Plan, Time_OTK_G_Actual, Time_OTK_E_Actual, Is_OTK_G_NotNeed, Is_OTK_E_NotNeed,
                            options, COM_Package_Type, IsStop);

                        if (drClient == null)
                            LogManager.LogError(unit, "(LoadProject) Ошибка структуры файла: " + file + ". Клиент № " + clientId + " не найден", null);
                        else
                            ds.ClientProject.AddClientProjectRow(drClient, drProjects);

                        if (drProduct == null)
                            LogManager.LogError(unit, "(LoadProject) Ошибка структуры файла: " + file + ". Продукт № " + productId + " не найден", null);
                        else
                            ds.ProjectProduct.AddProjectProductRow(drProjects, drProduct);

                        //e = projectXml.Root.Element("Options");
                        //if (e != null)
                        //{
                        //    string[] opts = e.Value.Split(',');
                        //    foreach (var opt in opts.Select(s => s.Trim()).Where(s => s.Length > 0))
                        //    {
                        //        AgrDataSet.OptionRow drOption = ds.Option.FindByName(opt);
                        //        if (drOption == null)
                        //            drOption = ds.Option.AddOptionRow(opt, "");
                        //        AgrDataSet.ProjectOptionRow drProjectOption = ds.ProjectOption.
                        //            AddProjectOptionRow(drProjects.Id, drProduct.Id, drOption);
                        //    }
                        //}
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.LogError(unit, "(LoadProject) Ошибка структуры файла: " + file, ex);
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, "(LoadProject) Перемещение плохого проекта: " + file, ex);
                try
                {
                    LogManager.LogInfo(unit, string.Format("(LoadProject) Загрузка проекта {0} файл: {1}", id, file));
                    Directory.CreateDirectory(AgrPath + "\\Errors\\");
                    Directory.Move(AgrPath + "\\Projects\\" + id, AgrPath + "\\Errors\\" + id);
                }
                catch (System.Exception e)
                {
                    LogManager.LogError(unit, "(LoadProject) Перемещение плохого проекта: " + file, ex);
                }
                return false;
            }
        }

        public bool SaveProject(AgrDataSet ds, int id)
        {
            try
            {
                LogManager.LogInfo(unit, string.Format("(SaveProject) Сохранение проекта {0}", id));

                System.IO.Directory.CreateDirectory(AgrPath + "\\Projects\\" + id);
                AgrDataSet.ProjectsRow drProjects = ds.Projects.FindById(id);
                AgrDataSet.ClientProjectRow[] drClientProject = drProjects.GetClientProjectRows();
                if (drClientProject.Length != 1)
                    return false;

                AgrDataSet.ProjectProductRow[] drsProductProject = drProjects.GetProjectProductRows();
                if (drsProductProject.Length != 1)
                    return false;

                AgrDataSet.ProjectRow[] drsProject = drProjects.GetProjectRows();
                if (drsProject.Length != 1)
                    return false;

                string file = AgrPath + "\\Projects\\" + id + "\\date.txt";
                System.IO.TextWriter sw = new System.IO.StreamWriter(file);
                sw.WriteLine(System.DateTime.Now);
                if (drProjects.TimeEndActual != System.DateTime.MinValue)
                    sw.WriteLine(drProjects.TimeEndActual);
                sw.Close();

                AgrDataSet.ProjectRow drProject = drsProject[0];
                var project = new XElement("project",
                                           new XAttribute("Client", drClientProject[0].ClientId),
                                           new XAttribute("Product", drsProductProject[0].ProductId),
                                           new XAttribute("Name", drProject.Name),
                                           new XAttribute("IsManagerSetPlanDate", drProject.IsManagerSetPlanDate),
                                           new XAttribute("CustomerName", drProject.CustomerName));

                //AgrDataSet.ProjectProductRow drProjectProduct = ds.ProjectProduct.
                //    FindByProjectIdProductId(id, drsProductProject[0].ProductId);

                //AgrDataSet.ProjectOptionRow[] drProjectOption = drProjectProduct.GetProjectOptionRows();
                //var sb = new StringBuilder();
                //foreach (var optionStr in drProjectOption.Select(s => s.OptionName))
                //{
                //    sb.Append(optionStr);
                //    sb.Append(", ");
                //}
                //if (sb.Length > 0)
                //    project.Add(new XElement("Options")
                //        {
                //            Value = sb.ToString()
                //        });
                if (!string.IsNullOrEmpty(drProject.Options))
                {
                    project.Add(new XElement("Options")
                    {
                        Value = drProject.Options
                    });
                }
                if (!string.IsNullOrEmpty(drProject.COM_Package_Type))
                {
                    project.Add(new XElement("COM_Package_Type")
                    {
                        Value = drProject.COM_Package_Type
                    });
                }
                if (drProject.IsStop)
                {
                    project.Add(new XElement("IsStop")
                    {
                        Value = drProject.IsStop.ToString()
                    });
                }
                #region ITO
                bool eS = false;
                var e = new XElement("ITO");
                if (drProject.Time_ITO_G_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_ITO_G_Plan", drProject.Time_ITO_G_Plan));
                    eS = true;
                }
                if (drProject.Time_ITO_G_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_ITO_G_Actual", drProject.Time_ITO_G_Actual));
                    eS = true;
                }
                if (drProject.Time_ITO_E_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_ITO_E_Plan", drProject.Time_ITO_E_Plan));
                    eS = true;
                }
                if (drProject.Time_ITO_E_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_ITO_E_Actual", drProject.Time_ITO_E_Actual));
                    eS = true;
                }
                if (drProject.Time_ITO_R_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_ITO_R_Plan", drProject.Time_ITO_R_Plan));
                    eS = true;
                }
                if (drProject.Time_ITO_R_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_ITO_R_Actual", drProject.Time_ITO_R_Actual));
                    eS = true;
                }
                if (drProject.ITO_R_Mode.Length > 0)
                {
                    e.Add(new XAttribute("ITO_R_Mode", drProject.ITO_R_Mode));
                    eS = true;
                }
                if (drProject.Is_ITO_G_NotNeed)
                {
                    e.Add(new XAttribute("Is_ITO_G_NotNeed", drProject.Is_ITO_G_NotNeed));
                    eS = true;
                }
                if (drProject.Is_ITO_E_NotNeed)
                {
                    e.Add(new XAttribute("Is_ITO_E_NotNeed", drProject.Is_ITO_E_NotNeed));
                    eS = true;
                }
                if (drProject.Is_ITO_R_NotNeed)
                {
                    e.Add(new XAttribute("Is_ITO_R_NotNeed", drProject.Is_ITO_R_NotNeed));
                    eS = true;
                }
                if (eS)
                    project.Add(e);

                #endregion ITO

                #region WH

                eS = false;
                e = new XElement("WH");
                if (drProject.Time_WH_G_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_G_Plan", drProject.Time_WH_G_Plan));
                    eS = true;
                }
                if (drProject.Time_WH_G_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_G_Actual", drProject.Time_WH_G_Actual));
                    eS = true;
                }
                if (drProject.Time_WH_E_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_E_Plan", drProject.Time_WH_E_Plan));
                    eS = true;
                }
                if (drProject.Time_WH_E_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_E_Actual", drProject.Time_WH_E_Actual));
                    eS = true;
                }
                if (drProject.Time_WH_R_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_R_Actual", drProject.Time_WH_R_Actual));
                    eS = true;
                }
                if (drProject.Time_WH_R_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_R_Plan", drProject.Time_WH_R_Plan));
                    eS = true;
                }

                if (drProject.Is_WH_G_NotNeed)
                {
                    e.Add(new XAttribute("Is_WH_G_NotNeed", drProject.Is_WH_G_NotNeed));
                    eS = true;
                }
                if (drProject.Is_WH_E_NotNeed)
                {
                    e.Add(new XAttribute("Is_WH_E_NotNeed", drProject.Is_WH_E_NotNeed));
                    eS = true;
                }
                if (drProject.Is_WH_R_NotNeed)
                {
                    e.Add(new XAttribute("Is_WH_R_NotNeed", drProject.Is_WH_R_NotNeed));
                    eS = true;
                }

                if (drProject.Time_WH_E_Requests_Create != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_E_Requests_Create", drProject.Time_WH_E_Requests_Create));
                    eS = true;
                }
                if (drProject.Time_WH_G_Requests_Create != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_WH_G_Requests_Create", drProject.Time_WH_G_Requests_Create));
                    eS = true;
                }
                if (eS)
                    project.Add(e);

                #endregion WH

                #region OMTS

                eS = false;
                e = new XElement("OMTS");
                if (drProject.Time_OMTS_G_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_OMTS_G_Plan", drProject.Time_OMTS_G_Plan));
                    eS = true;
                }
                if (drProject.Time_OMTS_G_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_OMTS_G_Actual", drProject.Time_OMTS_G_Actual));
                    eS = true;
                }
                if (drProject.Time_OMTS_E_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_OMTS_E_Plan", drProject.Time_OMTS_E_Plan));
                    eS = true;
                }
                if (drProject.Time_OMTS_E_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_OMTS_E_Actual", drProject.Time_OMTS_E_Actual));
                    eS = true;
                }

                if (eS)
                    project.Add(e);

                #endregion OMTS

                #region Time

                eS = false;
                e = new XElement("Time");
                if (drProject.TimeBegin != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("TimeBegin", drProject.TimeBegin));
                    eS = true;
                }
                if (drProject.TimeEndPlaned != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("TimeEndPlaned", drProject.TimeEndPlaned));
                    eS = true;
                }
                if (drProject.Com_New_Time != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Com_New_Time", drProject.Com_New_Time));
                    eS = true;
                }
                if (eS)
                    project.Add(e);

                #endregion Time

                #region MF

                eS = false;
                e = new XElement("MF");
                if (drProject.MF_Level.Length > 0)
                {
                    e.Add(new XAttribute("MF_Level", drProject.MF_Level));
                    eS = true;
                }
                if (drProject.MF_Rama.Length > 0)
                {
                    e.Add(new XAttribute("MF_Rama", drProject.MF_Rama));
                    eS = true;
                }
                if (drProject.MF_Post.Length > 0)
                {
                    e.Add(new XAttribute("MF_Post", drProject.MF_Post));
                    eS = true;
                }
                if (drProject.MF_Agregat.Length > 0)
                {
                    e.Add(new XAttribute("MF_Agregat", drProject.MF_Agregat));
                    eS = true;
                }
                if (drProject.MF_SH_Place.Length > 0)
                {
                    e.Add(new XAttribute("MF_SH_Place", drProject.MF_SH_Place));
                    eS = true;
                }
                if (drProject.MF_SH.Length > 0)
                {
                    e.Add(new XAttribute("MF_SH", drProject.MF_SH));
                    eS = true;
                }
                if (drProject.MF_Test)
                {
                    e.Add(new XAttribute("MF_Test", drProject.MF_Test));
                    eS = true;
                }

                if (drProject.MF_Time_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("MF_Time_Plan", drProject.MF_Time_Plan));
                    eS = true;
                }
                if (drProject.MF_Time_Test_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("MF_Time_Test_Plan", drProject.MF_Time_Test_Plan));
                    eS = true;
                }
                if (drProject.MF_Time_Test_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("MF_Time_Test_Actual", drProject.MF_Time_Test_Actual));
                    eS = true;
                }
                if (drProject.MF_Time != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("MF_Time", drProject.MF_Time));
                    eS = true;
                }
                if (drProject.MF_Collector.Length > 0)
                {
                    e.Add(new XAttribute("MF_Collector", drProject.MF_Collector));
                    eS = true;
                }
                if (drProject.MF_Complete_Percentage.Length > 0)
                {
                    e.Add(new XAttribute("MF_Complete_Percentage", drProject.MF_Complete_Percentage));
                    eS = true;
                }
                if (eS)
                    project.Add(e);

                #endregion MF

                #region OTK

                eS = false;
                e = new XElement("OTK");
                if (drProject.Time_OTK_Plan != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_OTK_Plan", drProject.Time_OTK_Plan));
                    eS = true;
                }
                if (drProject.Time_OTK_G_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_OTK_G_Actual", drProject.Time_OTK_G_Actual));
                    eS = true;
                }
                if (drProject.Time_OTK_E_Actual != System.DateTime.MinValue)
                {
                    e.Add(new XAttribute("Time_OTK_E_Actual", drProject.Time_OTK_E_Actual));
                    eS = true;
                }

                if (drProject.Is_OTK_G_NotNeed)
                {
                    e.Add(new XAttribute("Is_OTK_G_NotNeed", drProject.Is_OTK_G_NotNeed));
                    eS = true;
                }
                if (drProject.Is_OTK_E_NotNeed)
                {
                    e.Add(new XAttribute("Is_OTK_E_NotNeed", drProject.Is_OTK_E_NotNeed));
                    eS = true;
                }
                if (eS)
                    project.Add(e);

                #endregion OTK

                var projectXml = new XDocument(project);
                file = AgrPath + "\\Projects\\" + id + "\\info.xml";
                projectXml.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(SaveProject) Ошибка сохранения проекта: {0}", id), ex);
                return false;
            }
        }

        public bool LoadComments(AgrDataSet ds, int projectId)
        {
            AgrDataSet.ProjectsRow drProjects = ds.Projects.FindById(projectId);
            string file = AgrPath + "\\Projects\\" + projectId + "\\comments.xml";

            LogManager.LogInfo(unit, string.Format("(LoadComments) Загрузка коментариев проекта {0}: {1}", projectId, file));

            if (!System.IO.File.Exists(file))
                return false;

            try
            {
                XDocument projectXml = XDocument.Load(file);
                if (projectXml.Root != null)
                {
                    foreach (XElement e in projectXml.Root.Elements())
                    {
                        try
                        {
                            XAttribute attr = e.Attribute("Time");
                            if (attr == null)
                                continue;
                            System.DateTime time = System.DateTime.Parse(attr.Value);

                            attr = e.Attribute("User");
                            if (attr == null)
                                continue;
                            string user = attr.Value;
                            AgrDataSet.UserRow drUser = ds.User.FindByName(user);
                            if (drUser == null)
                                ds.User.AddUserRow(user, "", "", 0);

                            ds.Comment.AddCommentRow(drProjects, time, e.Value, drUser);
                        }
                        catch (System.Exception ex)
                        {
                            LogManager.LogError(unit, string.Format("(LoadComments) Ошибка загрузки структуры файла: {0}", file), ex);
                        }
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(LoadComments) Ошибка загрузки файла: {0}", file), ex);
                return false;
            }
        }

        public bool RemoveComment(int projectId, System.DateTime time, string message, string userName)
        {
            string file = AgrPath + "\\Projects\\" + projectId + "\\comments.xml";

            LogManager.LogInfo(unit, string.Format("(RemoveComment) Удаление коментария проекта {0}: {1}", projectId, file));
            try
            {
                if (System.IO.File.Exists(file))
                {
                    try
                    {
                        XDocument projectXml = XDocument.Load(file);
                        XElement root = projectXml.Root;

                        var xComment = new XElement("comment", new XAttribute("Time", time), new XAttribute("User", userName), new XCData(message));
                        if (root == null)
                            return true;

                        bool res = false;
                        foreach (XElement e in root.Elements())
                        {
                            XAttribute xTime = e.Attribute("Time");
                            if (xTime == null)
                                continue;
                            System.DateTime dt = System.DateTime.Parse(xTime.Value);
                            if (dt != time)
                                continue;
                            XAttribute xUser = e.Attribute("User");
                            if (xUser == null)
                                continue;
                            string user = xUser.Value;
                            if (user != userName)
                                continue;

                            e.Remove();
                            res = true;
                            break;
                        }
                        if (res)
                        {
                            var projectXmlNew = new XDocument(root);
                            projectXmlNew.Save(file);
                        }
                        return true;
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.LogError(unit, string.Format("(RemoveComment) Ошибка структуры файла: {0} при удаления комментария: {1}", file, message), ex);
                        return true;
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(RemoveComment) Ошибка файла: {0} при удаления комментария: {1}", file, message), ex);
                return false;
            }
        }

        public bool ClearComment(int projectId)
        {
            string file = AgrPath + "\\Projects\\" + projectId + "\\comments.xml";

            LogManager.LogInfo(unit, string.Format("(ClearComment) Очистка коментариев проекта {0}: {1}", projectId, file));
            try
            {
                System.IO.File.Delete(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(ClearComment) Ошибка удаления комментариев из файла: {0} для проекта: {1}", projectId, file), ex);
                return false;
            }
        }
        public bool AddComment(int projectId, System.DateTime time, string message, string userName)
        {
            string file = AgrPath + "\\Projects\\" + projectId + "\\comments.xml";
            LogManager.LogInfo(unit, string.Format("(AddComment) Добавление коментариев проекта {0}: {1}", projectId, file));
            try
            {
                XElement root;
                if (System.IO.File.Exists(file))
                {
                    try
                    {
                        XDocument projectXml = XDocument.Load(file);
                        root = projectXml.Root;
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.LogError(unit, string.Format("(AddComment) Ошибка загрузки комментария в файл: {0} для проекта: {1} ({2})", projectId, file, message), ex);
                        root = null;
                    }
                }
                else
                    root = null;

                var xComment = new XElement("comment",
                                            new XAttribute("Time", time),
                                            new XAttribute("User", userName),
                                            new XCData(message));
                if (root == null)
                    root = new XElement("comments");
                root.Add(xComment);

                var projectXmlNew = new XDocument(root);
                projectXmlNew.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(AddComment) Ошибка добавления комментария в файл: {0} для проекта: {1} ({2})", projectId, file, message), ex);

                return false;
            }
        }
        public bool SaveComment(AgrDataSet ds, int id)
        {
            string file = AgrPath + "\\Projects\\" + id + "\\comments.xml";

            LogManager.LogInfo(unit, string.Format("(SaveComment) Сохранение коментариев проекта {0}: {1}", id, file));
            try
            {
                if (!System.IO.Directory.Exists(AgrPath + "\\Projects\\" + id))
                    System.IO.Directory.CreateDirectory(AgrPath + "\\Projects\\" + id);

                var drsComment = (AgrDataSet.CommentRow[])ds.Comment.Select("ProjectId = " + id);
                var root = new XElement("comments");
                foreach (AgrDataSet.CommentRow drRequest in drsComment)
                {
                    var xComment = new XElement("comment",
                                            new XAttribute("Time", drRequest.Time),
                                            new XAttribute("User", drRequest.UserName),
                                            new XCData(drRequest.Value));
                    root.Add(xComment);
                }
                var projectXmlNew = new XDocument(root);
                projectXmlNew.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(SaveComment) Ошибка добавления комментария в файл: {0} для проекта: {1}", file, id), ex);
                return false;
            }
        }

        public bool LoadRequest(AgrDataSet ds, int projectId)
        {
            AgrDataSet.ProjectsRow drProjects = ds.Projects.FindById(projectId);
            string file = AgrPath + "\\Projects\\" + projectId + "\\request.xml";
            LogManager.LogInfo(unit, string.Format("(LoadRequest) Загрузка недокомплекта проекта {0}: {1}", projectId, file));
            if (!System.IO.File.Exists(file))
                return false;
            try
            {
                XDocument projectXml = XDocument.Load(file);
                if (projectXml.Root != null)
                {
                    foreach (XElement e in projectXml.Root.Elements())
                    {
                        XAttribute attr = e.Attribute("Article");
                        if (attr == null)
                            continue;
                        string article = attr.Value;

                        attr = e.Attribute("Name");
                        string name = (attr == null) ? "" : attr.Value;

                        attr = e.Attribute("Type");
                        int type = (attr == null) ? 0 : int.Parse(attr.Value);

                        attr = e.Attribute("TotalCount");
                        uint totalCount;
                        if (!uint.TryParse(attr.Value, out totalCount))
                            LogManager.LogError(unit, string.Format("(LoadRequest) Ошибка формата [TotalCount]:{0} для файла: {1}", attr.Value, file));

                        attr = e.Attribute("ExistCount");
                        uint existCount;
                        if (!uint.TryParse(attr.Value, out existCount))
                            LogManager.LogError(unit, string.Format("(LoadRequest) Ошибка формата [ExistCount]:{0} для файла: {1}", attr.Value, file));

                        attr = e.Attribute("DateComplete_Plan");
                        System.DateTime dateCompletePlan = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                        attr = e.Attribute("DateComplete");
                        System.DateTime dateComplete = attr == null ? System.DateTime.MinValue : System.DateTime.Parse(attr.Value);

                        attr = e.Attribute("IsCustomerMaterials");
                        bool isCustomerMaterials = false;
                        if (attr != null)
                            bool.TryParse(attr.Value, out isCustomerMaterials);

                        ds.Request.AddRequestRow(drProjects, article, name, type, dateCompletePlan, dateComplete, totalCount, existCount, isCustomerMaterials);

                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(LoadRequest) Ошибка загрузки недокомплекта проект: {0} файла: {1}", projectId, file), ex);
                return false;
            }
        }

        public bool AddRequest(int projectId, System.DateTime time, string message, string userName)
        {
            LogManager.LogInfo(unit, string.Format("(AddRequest) Добавление недокомплекта проекта {0}", projectId));
            return false;
        }
        public bool SaveRequest(AgrDataSet ds, int id)
        {
            string file = AgrPath + "\\Projects\\" + id + "\\request.xml";
            LogManager.LogInfo(unit, string.Format("(SaveRequest) Сохранение недокомплекта проекта {0}", id));
            try
            {
                var drsRequest = (AgrDataSet.RequestRow[])ds.Request.Select("ProjectId = " + id);
                var root = new XElement("requests");
                foreach (AgrDataSet.RequestRow drRequest in drsRequest)
                {
                    var xComment = new XElement("request",
                                                new XAttribute("Article", drRequest.Article),
                                                new XAttribute("Name", drRequest.Name),
                                                new XAttribute("Type", drRequest.Type),
                                                new XAttribute("TotalCount", drRequest.TotalCount),
                                                new XAttribute("ExistCount", drRequest.ExistCount),
                                                new XAttribute("IsCustomerMaterials", drRequest.IsCustomerMaterials));

                    if (drRequest.DateComplete_Plan != System.DateTime.MinValue)
                        xComment.Add(new XAttribute("DateComplete_Plan", drRequest.DateComplete_Plan));
                    if (drRequest.DateComplete != System.DateTime.MinValue)
                        xComment.Add(new XAttribute("DateComplete", drRequest.DateComplete));
                    root.Add(xComment);
                }
                var projectXmlNew = new XDocument(root);
                projectXmlNew.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(SaveRequest) Ошибка сохранения недокомплекта проект: {0} файла: {1}", id, file), ex);
                return false;
            }
        }

        //public bool LoadOptionsList(AgrDataSet ds)
        //{
        //    string file = AgrPath + "\\Options.xml";
        //    try
        //    {
        //        XDocument projectXml = XDocument.Load(file);
        //        if (projectXml.Root != null)
        //        {
        //            foreach (XElement e in projectXml.Root.Elements())
        //            {
        //                try
        //                {
        //                    XAttribute attr = e.Attribute("Name");
        //                    if (attr == null)
        //                        continue;
        //                    string name = attr.Value;

        //                    attr = e.Attribute("Description");
        //                    string description = attr == null ? "" : attr.Value;
        //                    ds.Option.AddOptionRow(name, description);
        //                }
        //                catch (System.Exception)
        //                {
        //                }
        //            }
        //        }
        //        return true;
        //    }
        //    catch (System.Exception)
        //    {
        //        return false;
        //    }
        //}

        public bool LoadProductList(AgrDataSet ds, bool archive)
        {
            string file = AgrPath + (archive ? ("\\Archive\\Products." + System.DateTime.Now.ToString("dd") + ".xml") : ("\\Products.xml"));
            LogManager.LogInfo(unit, string.Format("(LoadProductList) Загрузки списка изделий: {0}", file));
            try
            {
                XDocument projectXml = XDocument.Load(file);
                if (projectXml.Root != null)
                {
                    foreach (XElement e in projectXml.Root.Elements())
                    {
                        XAttribute attrId = e.Attribute("Id");
                        if (attrId == null)
                            continue;

                        int id = int.Parse(attrId.Value);
                        XAttribute attrName = e.Attribute("Name");
                        string name = attrName == null ? "" : attrName.Value;
                        attrName = e.Attribute("Description");
                        string description = attrName == null ? "" : attrName.Value;
                        ds.Product.AddProductRow(id, name, description);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(LoadProductList) Ошибка загрузки списка изделий файла: {0}", file), ex);

                if (!archive)
                {
                    System.IO.TextWriter sw = new System.IO.StreamWriter(AgrPath + "\\" + "BAD.txt", true);
                    sw.WriteLine("Сбой Products.xml: {0}", System.DateTime.Now);
                    sw.Close();

                    LoadProductList(ds, true);
                }

                return false;
            }
        }

        public bool SaveProductList(AgrDataSet ds)
        {
            string file = AgrPath + "\\Products.xml";
            LogManager.LogInfo(unit, string.Format("(SaveProductList) Сохранение списка изделий: : {0}", file));
            try
            {
                var dbs = new XElement("products");
                foreach (AgrDataSet.ProductRow drProduct in ds.Product.Select())
                {
                    var propUser = new XElement("products",
                                                new XAttribute("Id", drProduct.Id),
                                                new XAttribute("Name", drProduct.Name),
                                                new XAttribute("Description", drProduct.Description));
                    dbs.Add(propUser);
                }
                var projectXml = new XDocument(dbs);
                projectXml.Save(file);

                System.IO.Directory.CreateDirectory(AgrPath + "\\Archive\\");
                file = AgrPath + "\\Archive\\Products." + System.DateTime.Now.ToString("dd") + ".xml";
                projectXml.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(SaveProductList) Ошибка сохранения списка изделий файла: {0}", file), ex);
                return false;
            }
        }

        public bool LoadClientList(AgrDataSet ds, bool archive)
        {
            string file = AgrPath + (archive ? ("\\Archive\\Clients." + System.DateTime.Now.ToString("dd") + ".xml") : ("\\Clients.xml"));
            LogManager.LogInfo(unit, string.Format("(LoadClientList) Загрузка списка клиентов файл: {0}", file));
            try
            {
                XDocument projectXml = XDocument.Load(file);
                if (projectXml.Root != null)
                {
                    foreach (XElement e in projectXml.Root.Elements())
                    {

                        XAttribute attrId = e.Attribute("Id");
                        if (attrId == null)
                            continue;
                        int id = int.Parse(attrId.Value);

                        XAttribute attr = e.Attribute("Name");
                        string name = attr == null ? "" : attr.Value;

                        //attr = e.Attribute("Description");
                        //string decription = attr == null ? "" : attr.Value;

                        ds.Client.AddClientRow(id, name);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(LoadClientList) Ошибка загрузки списка клиентов файл: {0}", file), ex);

                if (!archive)
                {
                    System.IO.TextWriter sw = new System.IO.StreamWriter(AgrPath + "\\" + "BAD.txt", true);
                    sw.WriteLine("Сбой Clients.xml: {0}", System.DateTime.Now);
                    sw.Close();

                    LoadProductList(ds, true);
                }
                return false;
            }
        }

        public bool SaveClientList(AgrDataSet ds)
        {
            string file = AgrPath + "\\Clients.xml";
            LogManager.LogInfo(unit, string.Format("(SaveClientList) Сохранение списка клиентов файл: {0}", file));
            try
            {
                var dbs = new XElement("clients");
                foreach (AgrDataSet.ClientRow drClient in ds.Client.Select())
                {
                    var propUser = new XElement("client",
                                                new XAttribute("Id", drClient.Id),
                                                new XAttribute("Name", drClient.Name));
                    //new XAttribute("Description", drClient.Description));
                    dbs.Add(propUser);
                }
                var projectXml = new XDocument(dbs);

                projectXml.Save(file);

                System.IO.Directory.CreateDirectory(AgrPath + "\\Archive\\");
                file = AgrPath + "\\Archive\\Clients." + System.DateTime.Now.ToString("dd") + ".xml";
                projectXml.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(SaveClientList) Ошибка Сохранения списка клиентов файл: {0}", file), ex);
                return false;
            }
        }

        public bool LoadUsers(AgrDataSet ds)
        {
            string file = AgrPath + "\\Users.xml";
            LogManager.LogInfo(unit, string.Format("(LoadUsers) Загрузка списка пользователей файл: {0}", file));
            try
            {
                ds.User.Clear();

                XDocument projectXml = XDocument.Load(file);
                if (projectXml.Root != null)
                {
                    foreach (XElement e in projectXml.Root.Elements("user"))
                    {
                        XAttribute attr = e.Attribute("Name");
                        if (attr == null)
                            continue;
                        string name = attr.Value;

                        attr = e.Attribute("Login");
                        string login = attr == null ? "" : attr.Value;

                        attr = e.Attribute("Group");
                        string group = attr == null ? "" : attr.Value;

                        attr = e.Attribute("Right");
                        uint right = attr == null ? 0 : uint.Parse(attr.Value);

                        ds.User.AddUserRow(name, login, group, right);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(LoadUsers) Ошибка загрузки списка пользователей файл: {0}", file), ex);
                return false;
            }
        }

        public bool SaveUsers(AgrDataSet ds, bool two)
        {
            string file = AgrPath + "\\Users.xml";
            LogManager.LogInfo(unit, string.Format("(SaveUsers) Сохранение списка пользователей файл: {0}", file));
            try
            {
                if (two)
                {
                    System.IO.Directory.CreateDirectory(AgrPath);
                    System.IO.Directory.CreateDirectory(AgrPath + "\\Projects\\");
                }
                var dbs = new XElement("users");
                foreach (AgrDataSet.UserRow drUser in ds.User.Select())
                {
                    var propUser = new XElement("user",
                                                new XAttribute("Name", drUser.Name),
                                                new XAttribute("Login", drUser.Login),
                                                new XAttribute("Group", drUser.Group),
                                                new XAttribute("Right", drUser.Right));
                    dbs.Add(propUser);
                }
                var projectXml = new XDocument(dbs);
                projectXml.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, string.Format("(SaveUsers) Ошибка сохранения списка пользователей файл: {0}", file), ex);
                return false;
            }
        }

        #region Planner
        public bool LoadPlanners(AgrDataSet ds)
        {
            string file = AgrPath + "\\planners.xml";
            LogManager.LogInfo(unit, string.Format("(LoadPlanners) Загрузка списка постов файл: {0}", file));
            try
            {
                if (!File.Exists(file))
                    return true;

                XDocument projectXml = XDocument.Load(file);
                if (projectXml.Root != null)
                {
                    #region Post
                    XElement e = projectXml.Root.Element("Posts");
                    if (e != null)
                    {
                        foreach (XElement ePost in e.Elements("Post"))
                        {
                            XAttribute attr = ePost.Attribute("Id");
                            if (attr == null)
                                continue;
                            try
                            {
                                ds.Post.AddPostRow(int.Parse(attr.Value));
                            }
                            catch (System.Exception exc)
                            {
                                LogManager.LogError(unit, "(LoadPlanner) Ошибка загрузки списка постов. Файл: " + file, exc);
                            }
                        }
                    }
                    #endregion Post

                    #region Worker
                    e = projectXml.Root.Element("Workers");
                    if (e != null)
                    {
                        foreach (XElement ePost in e.Elements("Worker"))
                        {
                            XAttribute attr = ePost.Attribute("Post");
                            int post = attr == null ? 0 : int.Parse(attr.Value);
                            AgrDataSet.PostRow drPost = ds.Post.FindById(post);
                            if (drPost == null)
                            {
                                LogManager.LogError(unit,
                                    "(LoadPlanner) Ошибка загрузки списка сотрудников. Пост №" + post + " не найден");
                                continue;
                            }

                            attr = ePost.Attribute("Name");
                            string name = attr == null ? "" : attr.Value;

                            attr = ePost.Attribute("SecondName");
                            string secondName = attr == null ? "" : attr.Value;

                            attr = e.Attribute("EndWorkTime");
                            System.DateTime t = attr == null
                                ? System.DateTime.MinValue
                                : System.DateTime.Parse(attr.Value);

                            AgrDataSet.WorkerRow drWorker = ds.Worker.AddWorkerRow(drPost, name, secondName, t);

                            XElement eA = ePost.Element("Actions");
                            if (eA != null)
                            {
                                foreach (XElement eAction in eA.Elements("Action"))
                                {
                                    attr = eAction.Attribute("Type");
                                    if (attr == null)
                                    {
                                        LogManager.LogError(unit,
                                            "(LoadPlanner) Ошибка загрузки списка действий сотрудника. Не указано действие");
                                        continue;
                                    }
                                    try
                                    {
                                        var type = (MFWorkerActionType)System.Enum.Parse(typeof(MFWorkerActionType), attr.Value);
                                        attr = eAction.Attribute("Project");
                                        if (attr == null)
                                        {
                                            if (type == MFWorkerActionType.Project)
                                            {
                                                LogManager.LogError(unit,
                                                    "(LoadPlanner) Ошибка загрузки списка действий сотрудника. Не указан проект");
                                                continue;
                                            }
                                        }
                                        int projectId = int.Parse(attr.Value);
                                        AgrDataSet.ProjectsRow drProject;
                                        if (projectId == -1)
                                            drProject = null;
                                        else
                                        {
                                            drProject = ds.Projects.FindById(projectId);
                                            if (drProject == null)
                                            {
                                                LogManager.LogError(unit,
                                                    "(LoadPlanner) Ошибка загрузки списка действий сотрудников. Проект №" + projectId +
                                                    " не найден");
                                                continue;
                                            }
                                        }

                                        attr = eAction.Attribute("TimeBegin");
                                        System.DateTime timeBegin = attr == null
                                            ? System.DateTime.MinValue
                                            : System.DateTime.Parse(attr.Value);

                                        attr = eAction.Attribute("Days");
                                        int days = attr == null ? 0 : int.Parse(attr.Value);

                                        attr = eAction.Attribute("Comment");
                                        string comment = attr == null ? "" : attr.Value;


                                        //attr = ePost.Attribute("SecondName");
                                        //string secondName = attr == null ? "" : attr.Value;

                                        //attr = eAction.Attribute("EndWorkTime");
                                        //System.DateTime t = attr == null
                                        //    ? System.DateTime.MinValue
                                        //    : System.DateTime.Parse(attr.Value);

                                        ds.Action.AddActionRow(drProject, drWorker, timeBegin, days, comment, (int)type);
                                    }
                                    catch (System.Exception)
                                    {
                                        LogManager.LogError(unit,
                                          "(LoadPlanner) Ошибка загрузки списка действий сотрудника. Неизвестное действие");
                                    }
                                }
                            }
                        }
                    }
                    #endregion Worker
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, "(LoadPlanner) Ошибка загрузки файла: " + file, ex);
                return false;
            }
        }
        public bool SavePlanners(AgrDataSet ds)
        {
            string file = AgrPath + "\\planners.xml";
            LogManager.LogInfo(unit, string.Format("(SavePlanners) Сохранение списка постов файл: {0}", file));
            try
            {
                XElement root = new XElement("root");
                XElement e = new XElement("Posts");
                foreach (AgrDataSet.PostRow drPost in ds.Post)
                {
                    XElement ePost = new XElement("Post", new XAttribute("Id", drPost.Id));
                    e.Add(ePost);
                }
                root.Add(e);

                e = new XElement("Workers");
                foreach (AgrDataSet.WorkerRow drWorker in ds.Worker)
                {
                    XElement eWorker = new XElement("Worker",
                        new XAttribute("Name", drWorker.Name),
                        new XAttribute("SecondName", drWorker.SecondName),
                        new XAttribute("Post", drWorker.Post),
                        new XAttribute("EndWorkTime", drWorker.EndWorkTime));
                    XElement eA = new XElement("Actions");
                    foreach (AgrDataSet.ActionRow drAction in drWorker.GetActionRows())
                    {
                        XElement eAction = new XElement("Action",
                            new XAttribute("Type", ((MFWorkerActionType)drAction.Type)),
                            new XAttribute("Project", drAction.IsNull(ds.Action.TargetIdColumn) ? -1 : drAction.TargetId),
                            new XAttribute("TimeBegin", drAction.TimeBegin),
                            new XAttribute("Days", drAction.Days),
                            new XAttribute("Comment", drAction.Comment));
                        eA.Add(eAction);
                    }
                    eWorker.Add(eA);
                    e.Add(eWorker);
                }
                root.Add(e);
                var projectXml = new XDocument(root);
                projectXml.Save(file);
                return true;
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(unit, "(SavePlanners) Ошибка сохранения файла: " + file, ex);
                return false;
            }
        }
        #endregion Planner
    }
}
