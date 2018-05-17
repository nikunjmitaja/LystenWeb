namespace LystenApi.Utility
{
    public class ResultClass
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string AccessToken { get; set; }
        public dynamic Data { get; set; }
    }

    public class ResultClassForNonAuth
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }

    public class ResultClassCommon
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
    public class ResultClassToken
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string AccessToken { get; set; }
    }
}