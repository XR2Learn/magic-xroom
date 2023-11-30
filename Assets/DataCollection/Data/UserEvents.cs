
namespace DataCollection.Data
{ 
    public class UserEvents
    {
        public const string HEADER = "timestamp,event_type,info";

        public enum Scenario
        {
            SCENARIO_STARTED, SCENARIO_ENDED
        }

        public enum Level
        {
            LEVEL_STARTED, LEVEL_FAILED, LEVEL_COMPLETED
        }

        public enum Teleport
        {
            TELEPORT_IN, TELEPORT_OUT
        }

        public enum Feedback
        {
            BORED, ENGAGED, FRUSTRATED, SKIP
        }
    }
}
