namespace NoAcgNew.Onebot.Models.ApiParams
{
    public class EventReturn<T>
    {
        public int Code { get; set; }
        public T Return;
    }
}