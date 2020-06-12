using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    /// <summary>
    /// Defines insertion mode
    /// </summary>
    public enum InsertionMode
    {
        /// <summary>
        /// Replace the contents of the element.
        /// </summary>
        Replace = 0,

        /// <summary>
        /// Insert before the element.
        /// </summary>
        InsertBefore = 1,

        /// <summary>
        /// Insert after the element.
        /// </summary>
        InsertAfter = 2,

        /// <summary>
        /// Replace the entire element.
        /// </summary>
        ReplaceWith = 3
    }

    /// <summary>
    /// Ajax form options
    /// </summary>
    public class AjaxFormOptions
    {
        private string _confirm;
        private string _httpMethod;
        private string _loadingElementId;
        private string _onBegin;
        private string _onComplete;
        private string _onFailure;
        private string _onSuccess;
        private string _updateTargetId;
        private string _url;
        private const string InsertionModeReplaceString = "Sys.Mvc.InsertionMode.replace";
        private const string InsertionModeInsertBeforeString = "Sys.Mvc.InsertionMode.insertBefore";
        private const string InsertionModeInsertAfterString = "Sys.Mvc.InsertionMode.insertAfter";
        private const string InsertionModeUnobtrusiveReplace = "replace";
        private const string InsertionModeUnobtrusiveInsertBefore = "before";
        private const string InsertionModeUnobtrusiveInsertAfter = "after";
        private const string InsertionModeUnobtrusiveReplaceWith = "replace-with";
        private InsertionMode _insertionMode = InsertionMode.Replace;
        private static readonly Regex _idRegex = new Regex(@"[.:[\]]");

        /// <summary>
        /// Gets or sets whether confirm
        /// </summary>
        public string Confirm
        {
            get { return _confirm ?? string.Empty; }
            set { _confirm = value; }
        }

        /// <summary>
        /// Gets or sets the http method
        /// </summary>
        public string HttpMethod
        {
            get { return _httpMethod ?? string.Empty; }
            set { _httpMethod = value; }
        }

        /// <summary>
        /// Gets or sets the insertion mode
        /// </summary>
        public InsertionMode InsertionMode
        {
            get
            {
                return _insertionMode;
            }
            set
            {
                switch (value)
                {
                    case InsertionMode.Replace:
                    case InsertionMode.InsertAfter:
                    case InsertionMode.InsertBefore:
                    case InsertionMode.ReplaceWith:
                        _insertionMode = value;
                        return;
                    default:
                        throw new ArgumentException(nameof(value));
                }
            }
        }

        /// <summary>
        /// Gets insertion mode string
        /// </summary>
        internal string InsertionModeString
        {
            get
            {
                switch (InsertionMode)
                {
                    case InsertionMode.Replace:
                        return InsertionModeReplaceString;
                    case InsertionMode.InsertBefore:
                        return InsertionModeInsertBeforeString;
                    case InsertionMode.InsertAfter:
                        return InsertionModeInsertAfterString;
                    default:
                        return ((int)InsertionMode).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>
        /// Gets insertion model unobrusive string
        /// </summary>
        internal string InsertionModeUnobtrusive
        {
            get
            {
                switch (InsertionMode)
                {
                    case InsertionMode.Replace:
                        return InsertionModeUnobtrusiveReplace;
                    case InsertionMode.InsertBefore:
                        return InsertionModeUnobtrusiveInsertBefore;
                    case InsertionMode.InsertAfter:
                        return InsertionModeUnobtrusiveInsertAfter;
                    case InsertionMode.ReplaceWith:
                        return InsertionModeUnobtrusiveReplaceWith;
                    default:
                        return ((int)InsertionMode).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>
        /// Gets or sets the loading element duration
        /// </summary>
        public int LoadingElementDuration { get; set; }

        /// <summary>
        /// Gets or sets the loading element id
        /// </summary>
        public string LoadingElementId
        {
            get
            {
                return _loadingElementId ?? string.Empty;
            }
            set
            {
                _loadingElementId = value;
            }
        }

        /// <summary>
        /// Gets or sets the begin event
        /// </summary>
        public string OnBegin
        {
            get { return _onBegin ?? string.Empty; }
            set { _onBegin = value; }
        }

        /// <summary>
        /// Gets or sets the complete event
        /// </summary>
        public string OnComplete
        {
            get { return _onComplete ?? string.Empty; }
            set { _onComplete = value; }
        }

        /// <summary>
        /// Gets or sets the failure event
        /// </summary>
        public string OnFailure
        {
            get { return _onFailure ?? string.Empty; }
            set { _onFailure = value; }
        }

        /// <summary>
        /// Gets or sets the success event
        /// </summary>
        public string OnSuccess
        {
            get { return _onSuccess ?? string.Empty; }
            set { _onSuccess = value; }
        }

        /// <summary>
        /// Gets or sets the update target id
        /// </summary>
        public string UpdateTargetId
        {
            get { return _updateTargetId ?? string.Empty; }
            set { _updateTargetId = value; }
        }

        /// <summary>
        /// Gets or sets the url
        /// </summary>
        public string Url
        {
            get { return _url ?? string.Empty; }
            set { _url = value; }
        }

        /// <summary>
        /// Gets or sets whether allow to cache
        /// </summary>
        public bool AllowCache { get; set; }

        /// <summary>
        ///  Generage javascript string
        /// </summary>
        /// <returns></returns>
        internal string ToJavascriptString()
        {
            StringBuilder optionsBuilder = new StringBuilder("{");
            optionsBuilder.AppendFormat(CultureInfo.InvariantCulture, " insertionMode: {0},", InsertionModeString);
            optionsBuilder.Append(PropertyStringIfSpecified("confirm", Confirm));
            optionsBuilder.Append(PropertyStringIfSpecified("httpMethod", HttpMethod));
            optionsBuilder.Append(PropertyStringIfSpecified("loadingElementId", LoadingElementId));
            optionsBuilder.Append(PropertyStringIfSpecified("updateTargetId", UpdateTargetId));
            optionsBuilder.Append(PropertyStringIfSpecified("url", Url));
            optionsBuilder.Append(EventStringIfSpecified("onBegin", OnBegin));
            optionsBuilder.Append(EventStringIfSpecified("onComplete", OnComplete));
            optionsBuilder.Append(EventStringIfSpecified("onFailure", OnFailure));
            optionsBuilder.Append(EventStringIfSpecified("onSuccess", OnSuccess));
            optionsBuilder.Length--;
            optionsBuilder.Append(" }");
            return optionsBuilder.ToString();
        }

        /// <summary>
        /// Gets unobtrusive html attributes
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ToUnobtrusiveHtmlAttributes()
        {
            var result = new Dictionary<string, object>
            {
                { "data-ajax", "true" },
            };

            AddToDictionaryIfSpecified(result, "data-ajax-url", Url);
            AddToDictionaryIfSpecified(result, "data-ajax-method", HttpMethod);
            AddToDictionaryIfSpecified(result, "data-ajax-confirm", Confirm);

            AddToDictionaryIfSpecified(result, "data-ajax-begin", OnBegin);
            AddToDictionaryIfSpecified(result, "data-ajax-complete", OnComplete);
            AddToDictionaryIfSpecified(result, "data-ajax-failure", OnFailure);
            AddToDictionaryIfSpecified(result, "data-ajax-success", OnSuccess);

            if (AllowCache)
            {
                AddToDictionaryIfSpecified(result, "data-ajax-cache", "true");
            }

            if (!string.IsNullOrWhiteSpace(LoadingElementId))
            {
                result.Add("data-ajax-loading", EscapeIdSelector(LoadingElementId));

                if (LoadingElementDuration > 0)
                {
                    result.Add("data-ajax-loading-duration", LoadingElementDuration);
                }
            }

            if (!string.IsNullOrWhiteSpace(UpdateTargetId))
            {
                result.Add("data-ajax-update", EscapeIdSelector(UpdateTargetId));
                result.Add("data-ajax-mode", InsertionModeUnobtrusive);
            }

            return result;
        }

        /// <summary>
        /// Add value to dictionary
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        private static void AddToDictionaryIfSpecified(IDictionary<string, object> dictionary, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                dictionary.Add(name, value);
            }
        }

        /// <summary>
        /// Get event string
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="handler">handler</param>
        /// <returns></returns>
        private static string EventStringIfSpecified(string propertyName, string handler)
        {
            if (!string.IsNullOrEmpty(handler))
            {
                return string.Format(CultureInfo.InvariantCulture, " {0}: Function.createDelegate(this, {1}),", propertyName, handler.ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets property string
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        private static string PropertyStringIfSpecified(string propertyName, string propertyValue)
        {
            if (!string.IsNullOrEmpty(propertyValue))
            {
                string escapedPropertyValue = propertyValue.Replace("'", @"\'");
                return string.Format(CultureInfo.InvariantCulture, " {0}: '{1}',", propertyName, escapedPropertyValue);
            }
            return string.Empty;
        }

        /// <summary>
        /// Escape id selector
        /// </summary>
        /// <param name="selector">selector</param>
        /// <returns></returns>
        private static string EscapeIdSelector(string selector)
        {
            return '#' + _idRegex.Replace(selector, @"\$&");
        }
    }
}
