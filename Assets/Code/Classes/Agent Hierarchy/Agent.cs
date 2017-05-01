//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

/// <summary>
/// The base class for all player types and market.
/// Contains owned money and resources for an Agent.
/// </summary>
public abstract class Agent
{
    protected int money;
    protected ResourceGroup resources;

    /// <summary>
    /// Gets the owned resources for this agent
    /// </summary>
    /// <returns>The owned resources for this agent</returns>
    public ResourceGroup GetResources()
    {
        return resources;
    }

    /// <summary>
    /// Sets the owned resources of this agent to be equal to the value provided
    /// </summary>
    /// <param name="resourcesToSet">The ResourceGroup to set</param>
    public void SetResources(ResourceGroup resourcesToSet)
    {
        resources = resourcesToSet;
    }

    /// <summary>
    /// Gets this agents current money balance
    /// </summary>
    /// <returns>This agents current money balance</returns>
    public int GetMoney()
    {
        return money;   
    }

    /// <summary>
    /// Sets this agents current money balance to the value provided
    /// </summary>
    /// <param name="moneyToSet">The value to set this agents current money value to</param>
    public void SetMoney(int moneyToSet)
    {
        money = moneyToSet;
    }
}
