using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories;

// целесообразность использования Repository?
// частично избыточен из-за наличия в EfCore
// 1. используя отдельные Repository упрощает тестирование, DbContext тяжело тестировать
// 2. Протекаемая абстрация IQuerable, чтобы IQuerable дальше Domain не протекал, причина отладка кода
// Ef Core одна из ORM которой пользуются при разработке, Dapper, ADO net
// DbContext может приходит в Controller через сервис без репозитория, как правило в простых проектах
// для монолита применение паттерна Repository оправдано


// Task void проблема?
// при ошибке потеряю и ошибку и контекст никогда не возвращать void, а возвращать Task
// Task void до Core возможно было задокументировать и supressed exception, сейчас Core уходит в никуда,
// но есть возможность включить старое поведение 


// CancellationToken сигнал для завершения в случае отмены со стороны пользователя
// IsCanceletionRequest при работе с атомарными операциями, где можно каждую атомарную операцию проверить
// CanceletionException рекомеднуется при работе с потоковыми операциями
// CancellationTokenSource?

public interface IRepository<T>
where T : Entity
{
    /// <summary>
    /// Возвращает все элементы
    /// </summary>
    /// <returns></returns>
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(T entity, CancellationToken cancellationToken = default);
    Guid Update(T entity);
    void Delete(T entity);
}
