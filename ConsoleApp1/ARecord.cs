namespace ConsoleApp1
{
    public class ARecord
    {
        private static int _instanceCount = 0;

        public ARecord(int i)
        {
            this.InstanceCount = _instanceCount++;
            this.Index = i;
        }

        public override string ToString()
        {
            return $"{nameof(InstanceCount)}: {InstanceCount}";
        }

        public int InstanceCount { get; private set; }
        public int Index { get; }
    }
}