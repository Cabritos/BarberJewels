using UnityEngine;
using GooglePlayGames;

public class GoogleEventsReporter : MonoBehaviour
{
    public void ReportLevel (int levelNumber)
    {
        switch (levelNumber)
        {
            case 1:
                ReportEvent(GPGSIds.event_level_1);
                break;

            case 2:
                ReportEvent(GPGSIds.event_level_2);
                break;

            case 3:
                ReportEvent(GPGSIds.event_level_3);
                break;

            case 4:
                ReportEvent(GPGSIds.event_level_4);
                break;

            case 5:
                ReportEvent(GPGSIds.event_level_5);
                break;

            case 6:
                ReportEvent(GPGSIds.event_level_6);
                break;

            case 7:
                ReportEvent(GPGSIds.event_level_7);
                break;

            case 8:
                ReportEvent(GPGSIds.event_level_8);
                break;

            case 9:
                ReportEvent(GPGSIds.event_level_9);
                break;

            case 10:
                ReportEvent(GPGSIds.event_level_10);
                break;

            case 11:
                ReportEvent(GPGSIds.event_level_11);
                break;

            case 12:
                ReportEvent(GPGSIds.event_level_12);
                break;

            case 13:
                ReportEvent(GPGSIds.event_level_13);
                break;

            case 14:
                ReportEvent(GPGSIds.event_level_14);
                break;

            case 15:
                ReportEvent(GPGSIds.event_level_15);
                break;

            case 16:
                ReportEvent(GPGSIds.event_level_16);
                break;

            case 17:
                ReportEvent(GPGSIds.event_level_17);
                break;

            case 18:
                ReportEvent(GPGSIds.event_level_18);
                break;

            case 19:
                ReportEvent(GPGSIds.event_level_19);
                break;

            case 20:
                ReportEvent(GPGSIds.event_level_20);
                break;

            case 21:
                ReportEvent(GPGSIds.event_level_21);
                break;

            case 22:
                ReportEvent(GPGSIds.event_level_22);
                break;

            case 23:
                ReportEvent(GPGSIds.event_level_23);
                break;

            case 24:
                ReportEvent(GPGSIds.event_level_24);
                break;

            case 25:
                ReportEvent(GPGSIds.event_level_25);
                break;

            case 26:
                ReportEvent(GPGSIds.event_level_26);
                break;

            case 27:
                ReportEvent(GPGSIds.event_level_27);
                break;

            case 28:
                ReportEvent(GPGSIds.event_level_28);
                break;

            case 29:
                ReportEvent(GPGSIds.event_level_29);
                break;

            case 30:
                ReportEvent(GPGSIds.event_level_30);
                break;
        }
    }

    public void ReportIteration(int iterationNumber)
    {
        switch (iterationNumber)
        {
            case 0:
                ReportEvent(GPGSIds.event_iteration_0);
                break;

            case 1:
                ReportEvent(GPGSIds.event_iteration_1);
                break;

            case 2:
                ReportEvent(GPGSIds.event_iteration_2);
                break;

            case 3:
                ReportEvent(GPGSIds.event_iteration_3);
                break;

            case 4:
                ReportEvent(GPGSIds.event_iteration_4);
                break;

            case 5:
                ReportEvent(GPGSIds.event_iteration_5);
                break;

            case 6:
                ReportEvent(GPGSIds.event_iteration_6);
                break;

            case 7:
                ReportEvent(GPGSIds.event_iteration_7);
                break;

            case 8:
                ReportEvent(GPGSIds.event_iteration_8);
                break;

            case 9:
                ReportEvent(GPGSIds.event_iteration_9);
                break;

            case 10:
                ReportEvent(GPGSIds.event_iteration_10);
                break;

            case 11:
                ReportEvent(GPGSIds.event_iteration_11);
                break;

            case 12:
                ReportEvent(GPGSIds.event_iteration_12);
                break;

            case 13:
                ReportEvent(GPGSIds.event_iteration_13);
                break;

            case 14:
                ReportEvent(GPGSIds.event_iteration_14);
                break;

            case 15:
                ReportEvent(GPGSIds.event_iteration_15);
                break;

            case 16:
                ReportEvent(GPGSIds.event_iteration_16);
                break;

            case 17:
                ReportEvent(GPGSIds.event_iteration_17);
                break;

            case 18:
                ReportEvent(GPGSIds.event_iteration_18);
                break;

            case 19:
                ReportEvent(GPGSIds.event_iteration_19);
                break;

            case 20:
                ReportEvent(GPGSIds.event_iteration_20);
                break;

            case 21:
                ReportEvent(GPGSIds.event_iteration_21);
                break;

            case 22:
                ReportEvent(GPGSIds.event_iterartion_22);
                break;

            case 23:
                ReportEvent(GPGSIds.event_iteration_23);
                break;

            case 24:
                ReportEvent(GPGSIds.event_iteration_24);
                break;

            case 25:
                ReportEvent(GPGSIds.event_iteration_25);
                break;

            case 26:
                ReportEvent(GPGSIds.event_iteration_26);
                break;

            case 27:
                ReportEvent(GPGSIds.event_iteration_27);
                break;

            case 28:
                ReportEvent(GPGSIds.event_iteration_28);
                break;

            case 29:
                ReportEvent(GPGSIds.event_iteration_29);
                break;

            case 30:
                ReportEvent(GPGSIds.event_iteration_30);
                break;
        }
    }

    private void ReportEvent(string id)
    {
        if (!GameManager.Instance.ValidateConnectivity()) return;

        Debug.Log("Reporting event");
        PlayGamesPlatform.Instance.Events.IncrementEvent(id, 1);
    }
}