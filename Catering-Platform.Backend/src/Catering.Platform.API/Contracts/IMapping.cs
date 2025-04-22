namespace Catering.Platform.API.Contracts;

public interface IMapper<TEntity, TModel>
{
    TModel MapToModel(TEntity entity);
    TEntity MapToEntity(TModel model);
}