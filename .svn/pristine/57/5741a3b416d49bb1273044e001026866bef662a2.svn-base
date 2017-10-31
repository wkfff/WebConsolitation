using System;
using System.Linq;
using System.Linq.Expressions;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Data
{
    /// <summary>
    ///   Репозиторий для добавления ограничений на данные в 
    ///   соответствии с правами текущего пользователя
    /// </summary>
    /// <remarks>
    ///   Proxy- объект над другими реализациями ILinqRepository
    /// </remarks>
    /// <typeparam name="T"> Domain Object </typeparam>
    /// <example>
    ///   <code>ILinqRepository&lt;D_Org_UserProfile> repository;   
    ///     AuthService auth; 
    ///     ... 
    ///     var authConstraintRepository = 
    ///     new AuthRepositiory&lt;D_Org_UserProfile>(
    ///     repository, 
    ///     auth,
    ///     ppoIdExpr => ppoIdExpr.RefUchr.RefOrgPPO, 
    ///     grbsIdExpr => grbsIdExpr.RefUchr.RefOrgGRBS.ID,
    ///     orgIdExpr => orgIdExpr.RefUchr.ID);
    ///     ...
    ///     // Данные в выдаче с ограничениями, клиентский код без не изменений
    ///     authConstraintRepository.FindAll();</code>
    ///   <see cref="Krista.FM.RIA.Extensions.E86N.Auth.Presentation.Controllers.AccountsController" />
    /// </example>
    public sealed class AuthRepositiory<T> : ILinqRepository<T> where T : DomainObject
    {
        private readonly ILinqRepository<T> repository;

        private readonly ILinqSpecification<T> specification;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthRepositiory{T}"/> class. 
        ///   Основной конструктор. Строит спецификацию по умолчанию
        /// </summary>
        /// <param name="repository"> ILinqRepository с данными </param>
        /// <param name="auth"> сервис с данными  </param>
        /// <param name="ppoExpr"> выражение для подстановки ППО  </param>
        /// <param name="grbsExpr"> выражение для подстановки id ГРБС  </param>
        /// <param name="orgExpr"> выражение для подстановки id учереждения  </param>
        public AuthRepositiory(
            ILinqRepository<T> repository, 
            IAuthService auth, 
            Expression<Func<T, D_Org_PPO>> ppoExpr, 
            Expression<Func<T, int>> grbsExpr, 
            Expression<Func<T, int>> orgExpr)
        {
            this.repository = repository;
            specification = new Specification(auth, ppoExpr, grbsExpr, orgExpr);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthRepositiory{T}"/> class. 
        ///   Дополнительный конструктор. Используется спецификация клиента
        /// </summary>
        /// <param name="repository"> ILinqRepository с данными </param>
        /// <param name="specification"> ILinqSpecification со спецификацией </param>
        public AuthRepositiory(
            ILinqRepository<T> repository, 
            ILinqSpecification<T> specification)
        {
            this.repository = repository;
            this.specification = specification;
        }

        #region ILinqRepository<T> Members

        public IDbContext DbContext
        {
            get { return repository.DbContext; }
        }

        public IQueryable<T> FindAll(ILinqSpecification<T> linqSpecification)
        {
            return repository.FindAll(linqSpecification);
        }

        public T FindOne(ILinqSpecification<T> linqSpecification)
        {
            return repository.FindOne(linqSpecification);
        }

        /// <summary>Выборка со спецификацией по роли пользователя</summary>
        /// <remarks>FindAll() для клиента всегда будет возвращать  набор, ограниченный правами</remarks>
        public IQueryable<T> FindAll()
        {
            return repository.FindAll(specification);
        }

        public void Save(T entity)
        {
            repository.Save(entity);
        }

        public void SaveAndEvict(T entity)
        {
            repository.SaveAndEvict(entity);
        }

        public T Load(int id)
        {
            return repository.Load(id);
        }

        public T FindOne(int id)
        {
            return repository.FindAll(specification).Single(t => (t.ID == id));
        }

        public void Delete(T entity)
        {
            repository.Delete(entity);
        }

        #endregion

        #region Nested type: Specification

        /// <summary>
        ///   Default- спецификация для репозитория, ограничивающего выборку,
        ///   в соответствии с правами текущего пользователя
        /// </summary>
        private class Specification : ILinqSpecification<T>
        {
            private readonly IAuthService auth;

            private readonly Expression<Func<T, int>> grbsExpression;

            private readonly int grbsId;

            private readonly Expression<Func<T, int>> orgExpression;

            private readonly int orgId;

            private readonly string ppoCodePrefix;

            private readonly Expression<Func<T, D_Org_PPO>> ppoExpression;

            private readonly int ppoId;

            public Specification(
                IAuthService auth, 
                Expression<Func<T, D_Org_PPO>> ppoExpression, 
                Expression<Func<T, int>> grbsExpression, 
                Expression<Func<T, int>> orgExpression)
            {
                this.ppoExpression = ppoExpression;
                this.grbsExpression = grbsExpression;
                this.orgExpression = orgExpression;

                this.auth = auth;

                // сервис авторизации не доступен
                // дальнейшее смысла не имеет
                if (this.auth == null)
                {
                    // todo: требуется переопределить исключение
                    throw new NotImplementedException();
                }

                // администратору и наблюдателю все прощаем
                if (this.auth.IsAdmin() || this.auth.IsSpectator())
                {
                    return;
                }

                D_Org_UserProfile profile = this.auth.Profile;

                // непонятно кто это, и как сюда попал.
                // пока считаю нужным генерить исключение
                if (profile == null || !this.auth.IsRegisteredUser())
                {
                    // todo: требуется переопределить исключение
                    throw new NotImplementedException();
                }

                if (profile.RefUchr.RefOrgPPO != null)
                {
                    ppoId = profile.RefUchr.RefOrgPPO.ID;
                    ppoCodePrefix = profile.RefUchr.RefOrgPPO.Code.Substring(0, 5);
                }

                if (profile.RefUchr.RefOrgGRBS != null)
                {
                    grbsId = profile.RefUchr.RefOrgGRBS.ID;
                }

                orgId = profile.RefUchr.ID;
            }

            #region ILinqSpecification<T> Members

            public IQueryable<T> SatisfyingElementsFrom(IQueryable<T> candidates)
            {
                // для администратора и наблюдателя возвращаем полный набор
                if (auth.IsAdmin() || auth.IsSpectator())
                {
                    return candidates;
                }

                // todo: надо это говно как-то переделать. 
                // видимо надо тут все таки строить общее условие и потом применять его
                if (auth.IsPpoUser())
                {
                    return candidates.Where(
                        Expression.Lambda<Func<T, bool>>(
                            Expression.Call(
                                instance: Expression.Property(
                                    Expression.Invoke(ppoExpression, Expression.Parameter(typeof(T), "candidate")), 
                                    "Code"), 
                                method: typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), 
                                arguments: Expression.Constant(ppoCodePrefix, typeof(string))), 
                            Expression.Parameter(typeof(T), "candidate")));
                }

                if (auth.IsGrbsUser())
                {
                    IQueryable<T> result = WithAuthConstraint(
                        candidates,
                        Expression.Lambda<Func<T, int>>(
                            Expression.Property(Expression.Invoke(ppoExpression, Expression.Parameter(typeof(T), "candidate")), "ID"),
                            Expression.Parameter(typeof(T), "candidate")),
                        ppoId);
                    return WithAuthConstraint(result, grbsExpression, grbsId);
                }

                return WithAuthConstraint(candidates, orgExpression, orgId);
            }

            #endregion

            /// <summary>
            ///   Реализует ограничение вида IQueryable.Where(candidate => expression(candidate) == id)
            ///   данная форма корректно сереализует expression
            /// </summary>
            /// <param name="candidates"> IQueryable c данными </param>
            /// <param name="expression"> лямбда выражение </param>
            /// <param name="id"> значение ограничения </param>
            private static IQueryable<T> WithAuthConstraint(
                IQueryable<T> candidates, 
                Expression<Func<T, int>> expression, 
                int id)
            {
                return candidates.Where(
                    Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(
                            Expression.Invoke(expression, Expression.Parameter(typeof(T), "candidate")), 
                            Expression.Constant(id, typeof(int))), 
                        Expression.Parameter(typeof(T), "candidate")));
            }
        }

        #endregion
    }
}
