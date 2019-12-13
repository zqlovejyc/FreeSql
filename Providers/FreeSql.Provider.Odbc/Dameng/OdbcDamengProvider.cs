﻿using FreeSql.Internal;
using FreeSql.Internal.CommonProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace FreeSql.Odbc.Dameng
{

    public class OdbcDamengProvider<TMark> : IFreeSql<TMark>
    {

        public ISelect<T1> Select<T1>() where T1 : class => new OdbcDamengSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public ISelect<T1> Select<T1>(object dywhere) where T1 : class => new OdbcDamengSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
        public IInsert<T1> Insert<T1>() where T1 : class => new OdbcDamengInsert<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression);
        public IInsert<T1> Insert<T1>(T1 source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(T1[] source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(List<T1> source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(IEnumerable<T1> source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IUpdate<T1> Update<T1>() where T1 : class => new OdbcDamengUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public IUpdate<T1> Update<T1>(object dywhere) where T1 : class => new OdbcDamengUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
        public IDelete<T1> Delete<T1>() where T1 : class => new OdbcDamengDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public IDelete<T1> Delete<T1>(object dywhere) where T1 : class => new OdbcDamengDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);

        public IAdo Ado { get; }
        public IAop Aop { get; }
        public ICodeFirst CodeFirst { get; }
        public IDbFirst DbFirst { get; }
        public OdbcDamengProvider(string masterConnectionString, string[] slaveConnectionString)
        {
            this.InternalCommonUtils = new OdbcDamengUtils(this);
            this.InternalCommonExpression = new OdbcDamengExpression(this.InternalCommonUtils);

            this.Ado = new OdbcDamengAdo(this.InternalCommonUtils, masterConnectionString, slaveConnectionString);
            this.Aop = new AopProvider();

            this.DbFirst = new OdbcDamengDbFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);
            this.CodeFirst = new OdbcDamengCodeFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);

            //this.Aop.AuditValue += new EventHandler<Aop.AuditValueEventArgs>((_, e) =>
            //{
            //    if (e.Value == null && e.Column.Attribute.IsPrimary == false && e.Column.Attribute.IsIdentity == false)
            //        e.Value = Utils.GetDataReaderValue(e.Property.PropertyType.NullableTypeOrThis(), e.Column.Attribute.DbDefautValue);
            //});
        }

        internal CommonUtils InternalCommonUtils { get; }
        internal CommonExpression InternalCommonExpression { get; }

        public void Transaction(Action handler) => Ado.Transaction(handler);
        public void Transaction(TimeSpan timeout, Action handler) => Ado.Transaction(timeout, handler);
        public void Transaction(IsolationLevel isolationLevel, TimeSpan timeout, Action handler) => Ado.Transaction(isolationLevel, timeout, handler);

        public GlobalFilter GlobalFilter { get; } = new GlobalFilter();

        ~OdbcDamengProvider() => this.Dispose();
        int _disposeCounter;
        public void Dispose()
        {
            if (Interlocked.Increment(ref _disposeCounter) != 1) return;
            (this.Ado as AdoProvider)?.Dispose();
        }
    }
}
