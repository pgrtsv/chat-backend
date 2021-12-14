using System;

namespace ChatBackend
{
    /// <summary>
    /// Методы, помеченные этим атрибутом, не должны вызываться из пользовательского кода
    /// Вызов без явного подавления предупреждения должен приводить к ошибке сборки проекта
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class ForbidUsageAttribute : Attribute
    {
        // TODO: создать анализатор Roslyn, вызывающий ошибку сборки при использовании методов, помеченных этим аттрибутом.
    }
}