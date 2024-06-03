using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [SerializeField] private List<int> selectedCarIndices = new List<int>();
    private List<GameObject> allActiveGhosts = new List<GameObject>();
    // —ериализованные пол€ дл€ редактировани€ в инспекторе
    [SerializeField] private List<Transform> playerTransforms = new List<Transform>();
    [SerializeField] private List<GameObject> ghostPrefabs = new List<GameObject>();
    [SerializeField] private float recordInterval = 0.1f;
    [SerializeField] private float ghostDelay = 1.0f;

    // ѕриватные пол€
    private List<List<GhostData>> ghostDataLists = new List<List<GhostData>>();
    private List<List<GameObject>> activeGhostsList = new List<List<GameObject>>();
    private bool isShowingGhostTrail = false;

    private void Start()
    {
        InitializeLists();
        StartCoroutine(RecordPlayerPosition());
    }
    public void StopRecording()
    {
        StopAllCoroutines();
    }
    private void InitializeLists()
    {
        // »нициализаци€ списков данных привидений и активных привидений дл€ каждого игрока
        for (int i = 0; i < playerTransforms.Count; i++)
        {
            ghostDataLists.Add(new List<GhostData>());
            activeGhostsList.Add(new List<GameObject>());
        }
    }

    // ћетод дл€ сохранени€ следа игрока
    public void SavePlayerTrail()
    {
        StartCoroutine(RecordPlayerPosition());
    }
    
    private IEnumerator RecordPlayerPosition()
    {
        while (true)
        {
            for (int i = 0; i < playerTransforms.Count; i++)
            {
                ghostDataLists[i].Add(new GhostData(playerTransforms[i].position, playerTransforms[i].rotation));
            }

            yield return new WaitForSeconds(recordInterval);
        }
    }

    // ћетод дл€ показа следа привидени€ дл€ выбранного игрока
    public void ShowGhostTrail(List<int> selectedCarIndices)
    { 
        this.selectedCarIndices = selectedCarIndices;
        if (!isShowingGhostTrail)
        {
            StartCoroutine(SpawnGhostTrail());
        }
    }

    private IEnumerator SpawnGhostTrail()
    {
        isShowingGhostTrail = true;
        yield return new WaitForSeconds(ghostDelay);

        for (int i = 0; i < selectedCarIndices.Count; i++)
        {
            int selectedCarIndex = selectedCarIndices[i];
            if (selectedCarIndex >= 0 && selectedCarIndex < ghostPrefabs.Count)
            {
                GameObject ghostPrefab = ghostPrefabs[selectedCarIndex];
                GameObject ghost = Instantiate(ghostPrefab, ghostDataLists[selectedCarIndex][0].position, ghostDataLists[selectedCarIndex][0].rotation);
                activeGhostsList[selectedCarIndex].Add(ghost);

                for (int j = 1; j < ghostDataLists[selectedCarIndex].Count; j++)
                {
                    float elapsedTime = 0f;
                    float journeyTime = recordInterval;

                    while (elapsedTime < journeyTime)
                    {
                        if (j >= ghostDataLists[selectedCarIndex].Count - 1)
                        {
                            ghost.SetActive(false);
                            break;
                        }

                        float t = elapsedTime / journeyTime;
                        ghost.transform.position = Vector3.Lerp(ghostDataLists[selectedCarIndex][j - 1].position, ghostDataLists[selectedCarIndex][j].position, t);
                        ghost.transform.rotation = Quaternion.Slerp(ghostDataLists[selectedCarIndex][j - 1].rotation, ghostDataLists[selectedCarIndex][j].rotation, t);
                        elapsedTime += Time.deltaTime * Time.timeScale;
                        yield return null;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        isShowingGhostTrail = false;
    }
 
    // —труктура дл€ хранени€ данных привидени€
    private struct GhostData
    {
        public Vector3 position;
        public Quaternion rotation;

        public GhostData(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}