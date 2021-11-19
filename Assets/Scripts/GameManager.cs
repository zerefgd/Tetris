using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Grid { public int x, y; };

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject gamePiece;

    List<GameObject> currentPieces;
    Dictionary<Grid, GameObject> gridObjects;

    List<Grid> currentIndexes;
    List<Grid> tempIndexes;
    float waitTime;

    bool[,] fill = new bool[16,22];

    List<List<Grid>> pieces =
      new List<List<Grid>>()
      {
            new List<Grid>()
            {
                new Grid(){x = 1,y = 0},
                new Grid(){x = 1,y = 1},
                new Grid(){x = 1,y = 2},
                new Grid(){x = 1,y = 3},
            },
            new List<Grid>()
            {
                new Grid(){x = 0,y = 1},
                new Grid(){x = 0,y = 2},
                new Grid(){x = 1,y = 2},
                new Grid(){x = 1,y = 3},
            },
            new List<Grid>()
            {
                new Grid(){x = 1,y = 1},
                new Grid(){x = 1,y = 2},
                new Grid(){x = 0,y = 2},
                new Grid(){x = 0,y = 3},
            },
            new List<Grid>()
            {
                new Grid(){x = 1,y = 1},
                new Grid(){x = 1,y = 2},
                new Grid(){x = 0,y = 2},
                new Grid(){x = 1,y = 3},
            },
            new List<Grid>()
            {
                new Grid(){x = 0,y = 1},
                new Grid(){x = 1,y = 1},
                new Grid(){x = 1,y = 2},
                new Grid(){x = 1,y = 3},
            },
            new List<Grid>()
            {
                new Grid(){x = 1,y = 1},
                new Grid(){x = 1,y = 2},
                new Grid(){x = 1,y = 3},
                new Grid(){x = 0,y = 3},
            },
            new List<Grid>()
            {
                new Grid(){x = 0,y = 1},
                new Grid(){x = 1,y = 1},
                new Grid(){x = 0,y = 2},
                new Grid(){x = 1,y = 2},
            }
      };

    int direction;
    bool canRotate, hasGameFinished;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        currentIndexes = new List<Grid>();
        tempIndexes = new List<Grid>();
        currentPieces = new List<GameObject>();
        direction = 0;
        canRotate = false;
        hasGameFinished = false;
        waitTime = 0.3f;
        gridObjects = new Dictionary<Grid, GameObject>();

        int n = Random.Range(0, pieces.Count);

        for (int i = 0; i < pieces[n].Count; i++)
        {
            Grid tempGrid = pieces[n][i];
            tempGrid.x += 7;
            currentIndexes.Add(tempGrid);
            tempIndexes.Add(tempGrid);
        }

        for (int i = 0; i < currentIndexes.Count; i++)
        {
            GameObject temp = Instantiate(gamePiece);
            temp.transform.position = new Vector3(currentIndexes[i].x, -currentIndexes[i].y, -1f);
            currentPieces.Add(temp);
        }

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 22; j++)
            {
                fill[i, j] = false;
            }
        }

        StartCoroutine(UpdateTurn());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasGameFinished) return;
        if(Input.GetKeyDown(KeyCode.A))
        {
            direction = -1;
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            direction = 1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            canRotate = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            waitTime = 0.1f;
        }

    }

    IEnumerator UpdateTurn()
    {
        yield return new WaitForSeconds(waitTime);

        for (int i = 0; i < currentIndexes.Count; i++)
        {
            Grid tempGrid = currentIndexes[i];
            tempIndexes[i] = tempGrid;
            tempGrid.x += direction;
            currentIndexes[i] = tempGrid;
        }

        if(!Check())
        {
            for (int i = 0; i < currentIndexes.Count; i++)
            {
                currentIndexes[i] = tempIndexes[i];
            }
        }
        else
        {
            for (int i = 0; i < currentPieces.Count; i++)
            {
                currentPieces[i].transform.position = new Vector3(currentIndexes[i].x, -currentIndexes[i].y, -1f);
            }
        }


        if(canRotate)
        {
            Grid mid = currentIndexes[1];
            for (int i = 0; i < currentIndexes.Count; i++)
            {
                Grid tempGrid = currentIndexes[i];
                tempIndexes[i] = tempGrid;
                int x = tempGrid.y - mid.y;
                int y = tempGrid.x - mid.x;
                tempGrid.x = mid.x - x;
                tempGrid.y = mid.y + y;
                currentIndexes[i] = tempGrid;
            }

            if (Check())
            {
                for (int i = 0; i < currentPieces.Count; i++)
                {
                    currentPieces[i].transform.position = new Vector3(currentIndexes[i].x, -currentIndexes[i].y, -1f);
                }
            }
            else
            {
                for (int i = 0; i < currentIndexes.Count; i++)
                {
                    currentIndexes[i] = tempIndexes[i];
                }
            }
        }

        for (int i = 0; i < currentIndexes.Count; i++)
        {
            Grid tempGrid = currentIndexes[i];
            tempIndexes[i] = currentIndexes[i];
            tempGrid.y += 1;
            currentIndexes[i] = tempGrid;
        }

        if(!Check())
        {
            for (int i = 0; i < tempIndexes.Count; i++)
            {
                fill[tempIndexes[i].x, tempIndexes[i].y] = true;
                gridObjects[tempIndexes[i]] = currentPieces[i];
            }

            int n = Random.Range(0, pieces.Count);

            for (int i = 0; i < pieces[n].Count; i++)
            {
                Grid tempGrid = pieces[n][i];
                tempGrid.x += 7;
                currentIndexes[i] = tempGrid;
                tempIndexes[i] = tempGrid;
            }

            for (int i = 0; i < currentIndexes.Count; i++)
            {
                GameObject temp = Instantiate(gamePiece);
                temp.transform.position = new Vector3(currentIndexes[i].x, -currentIndexes[i].y, -1f);
                currentPieces[i]= temp;
            }

            if(!Check())
            {
                hasGameFinished = true;
                yield break;
            }

            waitTime = 0.3f;
        }
        else
        {
            for (int i = 0; i < currentPieces.Count; i++)
            {
                currentPieces[i].transform.position = new Vector3(currentIndexes[i].x, -currentIndexes[i].y, -1f);
            }
        }

        for (int i = 21; i > 0; i--)
        {
            if(CheckRow(i))
            {
                UpdateRow(i);
                i++;
            }
        }

        direction = 0;
        canRotate = false;

        StartCoroutine(UpdateTurn());
    }

    void UpdateRow(int i)
    {
        for (int j = 0; j < 16; j++)
        {
            GameObject tempObject;
            if(gridObjects.TryGetValue(new Grid() { x = j, y = i }, out tempObject))
            {
                tempObject.SetActive(false);
                gridObjects.Remove(new Grid() { x = j, y = i });
            }
        }

        for (int j = i; j > 0; j--)
        {
            for (int k = 0; k < 16; k++)
            {
                fill[k, j] = fill[k, j - 1];
            }
        }

        for (int k = 0; k < 16; k++)
        {
            fill[k, 0] = false;
        }

        Dictionary<Grid, GameObject> tempDictionary = new Dictionary<Grid, GameObject>();

        foreach (KeyValuePair<Grid,GameObject> pair in gridObjects)
        {
            Grid keyGrid = pair.Key;
            GameObject currentObject = pair.Value;
            if(keyGrid.y < i)
            {
                keyGrid.y += 1;
                currentObject.transform.position = new Vector3(keyGrid.x, -keyGrid.y, -1f);
            }
            tempDictionary[keyGrid] = currentObject;
        }

        gridObjects = tempDictionary;
    }

    bool CheckRow(int i)
    {
        for (int k = 0; k < 16; k++)
        {
            if (!fill[k, i]) return false;
        }
        return true;
    }

    bool Check()
    {
        for (int i = 0; i < currentIndexes.Count; i++)
        {
            if(currentIndexes[i].x < 0 || currentIndexes[i].x >= 16 || currentIndexes[i].y >= 22)
            {
                return false;
            }
            else if(fill[currentIndexes[i].x,currentIndexes[i].y])
            {
                return false; 
            }
        }
        return true;
    }

    public void GameRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
