using System;
using AutomataCLI.Extensions;

namespace AutomataCLI.Utils {
    public static class ValidationUtils {
        public static void EnsureNotNull(Object obj, Exception throwException) {
            if (obj == null) {
                throw throwException;
            }
        }

        public static void EnsureNull(Object obj, Exception throwException) {
            if (obj == null) {
                throw throwException;
            }
        }

        public static void EnsureNotDefault(Object obj, Exception throwException) {
            if (obj == obj.GetDefault()) {
                throw throwException;
            }
        }

        public static void EnsureDefault(Object obj, Exception throwException) {
            if (obj != obj.GetDefault()) {
                throw throwException;
            }
        }

        public static void EnsureNotNullEmptyOrWhitespace(String input, Exception throwException) {
            if (String.IsNullOrWhiteSpace(input)) {
                throw throwException;
            }
        }

        public static void EnsureNullEmptyOrWhitespace(String input, Exception throwException) {
            if (!String.IsNullOrWhiteSpace(input)) {
                throw throwException;
            }
        }
    }
}