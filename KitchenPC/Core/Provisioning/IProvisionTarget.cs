namespace KitchenPC.Provisioning
{
    using KitchenPC.Data;

    /// <summary>Represents a target which can be provisioned based on data exported from an IProvisionSource.</summary>
    public interface IProvisionTarget
    {
        void Import(IProvisionSource source);

        void InitializeStore();
    }
}