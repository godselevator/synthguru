using SynthGuru.DomainModel;

namespace SynthGuru.DataAccessLayer.Repositories
{

    #region *** Repository interfaces ***

    /// <summary>
    /// Manufacturer
    /// </summary>
    public interface IManufacturerRepository : IGenericDataRepository<Manufacturer>
    {
    }

    /// <summary>
    /// SynthesisType
    /// </summary>
    public interface ISynthesisTypeRepository : IGenericDataRepository<SynthesisType>
    {
    }

    /// <summary>
    /// SynthModel
    /// </summary>
    public interface ISynthModelRepository : IGenericDataRepository<SynthModel>
    {
    }

    #endregion

    #region *** Repository classes ***

    /// <summary>
    /// Manufacturer
    /// </summary>
    public class ManufacturerRepository : GenericDataRepository<Manufacturer>, IManufacturerRepository
    {
    }

    /// <summary>
    /// SynthesisType
    /// </summary>
    public class SynthesisTypeRepository : GenericDataRepository<SynthesisType>, ISynthesisTypeRepository
    {
    }

    /// <summary>
    /// SynthModel
    /// </summary>
    public class SynthModelRepository : GenericDataRepository<SynthModel>, ISynthModelRepository
    {
    }

    #endregion
}
