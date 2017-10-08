namespace RAGENativeUI
{
    using System;
    using System.Diagnostics;

    // Argument validation helper methods.
    // Add more as needed.
    [DebuggerStepThrough]
    internal static class Throw
    {
        public static void IfNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void IfNullOrEmpty(string value, string parameterName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException(parameterName);
            }
        }

        public static void IfNullOrWhiteSpace(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(parameterName);
            }
        }

        // inclusive
        public static void IfOutOfRange(int value, int min, int max, string parameterName, string message = null)
        {
            if(value < min || value > max)
            {
                throw message == null ? new ArgumentOutOfRangeException(parameterName) : new ArgumentOutOfRangeException(parameterName, message);
            }
        }

        public static void IfOutOfRange(decimal value, decimal min, decimal max, string parameterName, string message = null)
        {
            if (value < min || value > max)
            {
                throw message == null ? new ArgumentOutOfRangeException(parameterName) : new ArgumentOutOfRangeException(parameterName, message);
            }
        }

        public static void IfNegative(int value, string parameterName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, $"Negative values are invalid.");
            }
        }

        public static void IfNegative(float value, string parameterName)
        {
            if (value < 0.0f)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, $"Negative values are invalid.");
            }
        }

        public static void IfNegative(decimal value, string parameterName)
        {
            if (value < 0.0m)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, $"Negative values are invalid.");
            }
        }

        public static void InvalidOperationIf(bool value, string message = null)
        {
            if (value)
            {
                throw message == null ? new InvalidOperationException() : new InvalidOperationException(message);
            }
        }

        public static void InvalidOperationIfNot(bool value, string message = null)
        {
            if (!value)
            {
                throw message == null ? new InvalidOperationException() : new InvalidOperationException(message);
            }
        }

        public static void DisposedIf(bool value, string objectName = null, string message = null)
        {
            if (value)
            {
                throw new ObjectDisposedException(objectName, message);
            }
        }

        public static void DisposedIfNot(bool value, string objectName = null, string message = null)
        {
            if (!value)
            {
                throw new ObjectDisposedException(objectName, message);
            }
        }
    }
}

