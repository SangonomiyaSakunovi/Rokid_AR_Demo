namespace SceneBehaviours
{   
    internal interface ISceneBehaviour
    {
        string SceneBehaviourName { get; }
        void OnNewSceneOverlayed(string sceneBehaviourName);
    }
}
