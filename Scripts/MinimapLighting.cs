using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapLighting : MonoBehaviour
{
    [SerializeField, Child] Transform minimap_light;

    public Transform player_object;
    private Transform minimap_directions;

    private Transform minimap_north;
    private Transform minimap_south;
    private Transform minimap_east;
    private Transform minimap_west;

    private void Start()
    {
        minimap_directions = GatheredMinimapCompass.Instance.minimap_directions;
        minimap_north = GatheredMinimapCompass.Instance.minimap_north;
        minimap_south = GatheredMinimapCompass.Instance.minimap_south;
        minimap_east = GatheredMinimapCompass.Instance.minimap_east;
        minimap_west = GatheredMinimapCompass.Instance.minimap_west;

        Camera.onPreCull += camera_start;
        Camera.onPostRender += camera_done;
    }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void camera_start(Camera cam)
    {
        if (cam == Camera.main)
        {
            minimap_light.gameObject.SetActive(false);
        }
    }

    private void camera_done(Camera cam)
    {
        if (cam == Camera.main)
        {
            minimap_light.gameObject.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (player_object == null)
        {
            return;
        }

        minimap_directions.position = player_object.position;
        minimap_directions.localPosition = new Vector3(minimap_directions.localPosition.x, 280f, minimap_directions.localPosition.z);

        minimap_north.rotation = Quaternion.Euler(90f, 0f, -player_object.rotation.eulerAngles.y);
        minimap_south.rotation = Quaternion.Euler(90f, 0f, -player_object.rotation.eulerAngles.y);
        minimap_east.rotation = Quaternion.Euler(90f, 0f, -player_object.rotation.eulerAngles.y);
        minimap_west.rotation = Quaternion.Euler(90f, 0f, -player_object.rotation.eulerAngles.y);
    }
}
