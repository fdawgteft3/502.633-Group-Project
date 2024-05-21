using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApplication
{
    internal class SolarFlareController
    {
        private readonly SolarFlareView m_SolarFlareView;
        private readonly SolarFlareModel m_SolarFlareModel;
        private SolarFlareDataArr? m_SolarFlareDataArr;

        public SolarFlareController(SolarFlareModel model, SolarFlareView view)
        {
            m_SolarFlareModel = model ?? throw new ArgumentNullException(nameof(model));
            m_SolarFlareView = view ?? throw new ArgumentNullException(nameof(view));
        }

        public async Task RefreshSolarFlareData(string apiKey, string startDate, string endDate)
        {
            try
            {
                m_SolarFlareDataArr = await m_SolarFlareModel.GetSolarDataAsync(apiKey, startDate, endDate);
            }
            catch (WeatherServiceException ex)
            {
                ErrorLogger.Instance.LogError($"An error occurred while retrieving solar flare data: {ex.Message}");
            }
        }
        public void RefreshPanelView()
        {
            // Render the weather data only if it is available.
            if (null != m_SolarFlareDataArr)
            {
                m_SolarFlareView.Render(m_SolarFlareDataArr);
            }
        }

    }
}
