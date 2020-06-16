namespace ArgDb
{
    [System.Serializable]
    /// <summary>Описание запроса на поставку</summary>
    public class AgrRequest
    {
        #region Поля
        #endregion

        #region Свойства
        /// <summary>Тип недокомплекта</summary>
        public OMTSRequestType Type { get; set; }

        /// <summary>Всего нужно</summary>
        public uint TotalCount { get; set; }
        /// <summary>В наличие</summary>
        public uint ExistCount { get; set; }

        /// <summary>Название</summary>
        public string Name { get; set; }

        /// <summary>Давальческое сырье</summary>
        public bool IsCustomerMaterials { get; set; }

        /// <summary>Артикул товара</summary>
        public string Article { get; set; }

        /// <summary>Дата поставки (план)</summary>
        public System.DateTime? DateComplete_Plan { get; set; }
        /// <summary>Дата поставки</summary>
        public System.DateTime? DateComplete { get; set; }
        #endregion

        public AgrRequest(OMTSRequestType t, string name, uint count, string a = "")
        {
            Type = t;
            Name = name;
            TotalCount = count;
            Article = a;
        }
        public AgrRequest(OMTSRequestType t, string name)
        {
            Type = t;
            Name = name;
        }

        public bool IsSame(AgrRequest source)
        {
            return Type == source.Type
                && TotalCount == source.TotalCount
                && ExistCount == source.ExistCount
                && Name == source.Name
                && Article == source.Article
                && DateComplete_Plan == source.DateComplete_Plan
                && DateComplete == source.DateComplete
                && IsCustomerMaterials == source.IsCustomerMaterials;
        }

        public override string ToString()
        {
            return string.Format("{1}({0})", (Type == OMTSRequestType.Electrician ? "Электрика" : "Гидравлика"), Name);
        }
    }
}
