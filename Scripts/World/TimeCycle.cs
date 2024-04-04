using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets._Scripts.World
{
    public class TimeCycle
    {
        [System.Serializable]
        public struct TimeSection {
            public Color horizon_color;
            public Color zenith_color;
            public Color cloud_color;
            public Color sun_color;

            public Color ambient_Ground;
            public Color ambient_Sky;
            public Color ambient_Equator;

            public float cloud_alpha;
            public float sun_intensity;
            public float sun_light_intensity;


            public Vector3 sun_angle_start;
            public Vector3 sun_angle_end;

            public float time_start;
            public float duration;
            public float fade_duration;
        }

        /* Constants for the material properties we need to modify */
        private const string Zenith_Color_Key = "_Zenith_Color";
        private const string Horizon_Color_Key = "_Horizon_Color";
        private const string Sun_Color_Key = "_SunColorAddition";
        private const string Cloud_Color_Key = "_Cloud_Color";
        private const string Cloud_Alpha_Key = "_CloudAlpha";
        private const string Sun_Intensity_Key = "_SunIntensity";

        public float max_time = 0f;
        public List<TimeSection> sections = new List<TimeSection>();

        public void Add_Time_State(TimeSection new_time_section)
        {
            sections.Add(new_time_section);
        }

        public void Configure_Max_Time()
        {
            max_time = sections[sections.Count - 1].time_start + sections[sections.Count - 1].duration;
        }

        public TimeSection Get_Section_For_Time(float time, out TimeSection last_time_section)
        {
            // So we return the end if the current section is the first section in the list.
            TimeSection last_section = sections[sections.Count - 1];
            foreach (TimeSection section in sections)
            {
                if (time < section.time_start)
                {
                    last_section = section;
                    continue;
                }

                if (time >= section.time_start + section.duration)
                {
                    last_section = section;
                    continue;
                }

                last_time_section = last_section;
                return section;
            }

            Debug.LogError("Invalid Time!");
            Debug.Log($"{last_section.zenith_color}");
            last_time_section = last_section;
            return sections[0]; // Default to this if unknown Value. Shouldn't occur.
        }

        public void Set_Properties_For_Time(float time, Material skybox_material, Transform sun_transform, Light sun_light)
        {
            // Lots of hardcoded math, mostly lerps
            TimeSection last_section;
            TimeSection section = Get_Section_For_Time(time, out last_section);
            float fade_percent_progression = (time - section.time_start) / section.fade_duration;
            float sun_travel_percent_progression = (time - section.time_start) / section.duration;

            Color horizon_color = Color.Lerp(last_section.horizon_color, section.horizon_color, fade_percent_progression);
            Color zenith_color = Color.Lerp(last_section.zenith_color, section.zenith_color, fade_percent_progression);
            Color cloud_color = Color.Lerp(last_section.cloud_color, section.cloud_color, fade_percent_progression);
            Color sun_color = Color.Lerp(last_section.sun_color, section.sun_color, fade_percent_progression);

            Color ambient_Ground = Color.Lerp(last_section.ambient_Ground, section.ambient_Ground, fade_percent_progression);
            Color ambient_Sky = Color.Lerp(last_section.ambient_Sky, section.ambient_Sky, fade_percent_progression);
            Color ambient_Equator = Color.Lerp(last_section.ambient_Equator, section.ambient_Equator, fade_percent_progression);

            float cloud_alpha = Mathf.Lerp(last_section.cloud_alpha, section.cloud_alpha, fade_percent_progression);
            float sun_intensity = Mathf.Lerp(last_section.sun_intensity, section.sun_intensity, fade_percent_progression);
            float sun_light_intensity = Mathf.Lerp(last_section.sun_light_intensity, section.sun_light_intensity, fade_percent_progression);

            Vector3 sun_angle = Vector3.Lerp(section.sun_angle_start, section.sun_angle_end, sun_travel_percent_progression);

            // Actually set the properties
            skybox_material.SetColor(Horizon_Color_Key, horizon_color);
            skybox_material.SetColor(Zenith_Color_Key, zenith_color);
            skybox_material.SetColor(Cloud_Color_Key, cloud_color);
            skybox_material.SetColor(Sun_Color_Key, sun_color);

            skybox_material.SetFloat(Sun_Intensity_Key, sun_intensity);
            skybox_material.SetFloat(Cloud_Alpha_Key, cloud_alpha);

            RenderSettings.ambientEquatorColor = ambient_Equator;
            RenderSettings.ambientSkyColor = ambient_Sky;
            RenderSettings.ambientGroundColor = ambient_Ground;

            sun_transform.transform.rotation = Quaternion.Euler(sun_angle);
            sun_light.intensity = sun_light_intensity;
        }
    }
}
