using System;

namespace RobotsTxt {
    public sealed class RobotsTxtResult {
        public RobotsTxtResult(Memory<byte> content, Int32 maxAge) {
            Content = content;
            MaxAge = maxAge;
        }

        public Int32 MaxAge { get; }
        public Memory<byte> Content { get; }
    }
}
