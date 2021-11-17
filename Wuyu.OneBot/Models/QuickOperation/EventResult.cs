namespace Wuyu.OneBot.Models.QuickOperation
{
    public class EventResult<T> where T : BaseQuickOperation
    {
        public int Code { get; set; } = 0;

        public T Operation { get; set; }

        public EventResult()
        {

        }

        public EventResult(int code)
        {
            this.Code = code;
        }

        public static implicit operator EventResult<T>(int code) => new() { Code = code };

        public static implicit operator EventResult<T>((int code, T operation) data) => new() { Code = data.code, Operation = data.operation };
    }
}
