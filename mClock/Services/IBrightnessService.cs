using System;
namespace mClock.Services
{
    public interface IBrightnessService
    {
        void SetBrightness(float factor);

        float GetBrightness();
    }
}
