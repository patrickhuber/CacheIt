using System;
namespace CacheIt.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISegmentService
    {
        string GenerateSegmentKey(int segmentIndex, string key);
        int GetSegmentIndex(long position, int bufferSize);
        int GetPositionInSegment(long absolutePosition, int bufferSize);
    }
}
