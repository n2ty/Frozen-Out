using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Level.Mission
{
    public class MissionManager : MonoBehaviour
    {
        public List<MissionInfo> Missions;
        public List<MissionInfo> Submissions;

        public bool IsMissionDone(MissionBase mission)
        {
            MissionInfo missionInfo = GetMission(mission);

            return missionInfo.IsDone;
        }

        public bool IsSubmissionDone(MissionBase submission)
        {
            MissionInfo submissionInfo = GetSubmission(submission);

            return submissionInfo.IsDone;
        }

        public MissionInfo GetMission(MissionBase mission)
        {
            return Missions.Find(temp => temp.Equals(mission));
        }

        public MissionInfo GetSubmission(MissionBase submission)
        {
            return Submissions.Find(temp => temp.Equals(submission));
        }

        public MissionInfo GetActiveMission()
        {
            return Missions.Find(temp => temp.IsActive());
        }

        public void MarkMissionAsDone(MissionBase mission)
        {
            MissionInfo missionInfo = GetMission(mission);

            missionInfo.SetDone();

            int finishedIndex = Missions.IndexOf(missionInfo);
            int nextIndex = finishedIndex + 1;

            if (nextIndex < Missions.Count)
            {
                MissionInfo nextMission = Missions.ElementAt(nextIndex);
                nextMission.SetActive();
            }
        }

        public void MarkSubmissionAsDone(MissionBase mission)
        {
            MissionInfo submissionInfo = GetSubmission(mission);

            submissionInfo.SetDone();
        }
    }
}