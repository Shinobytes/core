/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Shinobytes.Core
{
    public class IoCContainer
    {
        private readonly Dictionary<Type, Type> registeredTypes = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> instantiatedTypes = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> instantiators = new Dictionary<Type, object>();

        public IoCContainer Register<TInterface, TImpl>() where TImpl : TInterface
        {
            var a = typeof(TInterface);
            var b = typeof(TImpl);
            if (registeredTypes.ContainsKey(a))
                registeredTypes[a] = b;
            else
                registeredTypes.Add(a, b);
            return this;
        }

        public IoCContainer RegisterCustom<TInterface>(Func<TInterface> instantiator)
        {
            Register<TInterface, TInterface>();
            var type = typeof(TInterface);
            if (instantiators.ContainsKey(type))
                instantiators[type] = instantiator;
            else
                instantiators.Add(type, instantiator);            
            return this;
        }

        public object Resolve(Type type)
        {
            var newType = registeredTypes[type];
            if (instantiatedTypes.ContainsKey(type))
                return instantiatedTypes[type];

            if (instantiators.ContainsKey(type))
            {
                dynamic func = instantiators[type];
                var newObj = func();
                instantiatedTypes.Add(type, newObj);
                return newObj;
            }

            var leastDemandingCtor = newType.GetConstructors()
                .OrderBy(i => i.GetParameters().Length)
                .FirstOrDefault(j => j.GetParameters().All(t => !t.ParameterType.IsPrimitive) && !j.IsStatic);

            if (leastDemandingCtor == null) throw new Exception($"Unable to instantiate the type '{type.FullName}', no suitable constructor was found.");
            var ctorParams = leastDemandingCtor.GetParameters();
            var values = ctorParams.Select(i => Resolve(i.ParameterType)).ToArray();
            var obj = Activator.CreateInstance(newType, values);
            instantiatedTypes.Add(type, obj);
            return obj;
        }

        public TInterface Resolve<TInterface>()
        {
            var type = typeof(TInterface);
            if (instantiatedTypes.ContainsKey(type)) return (TInterface)instantiatedTypes[type];
            if (!registeredTypes.ContainsKey(type)) throw new Exception($"Target type '{type.FullName}' was never registered before use.");
            return (TInterface)Resolve(type);
        }
    }
}