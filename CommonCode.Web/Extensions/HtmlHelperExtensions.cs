using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CommonCode.BusinessLayer.Helpers;
using CommonCode.Web.Enums;

namespace CommonCode.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString FormGroupCheckBoxHorizontalFor<TModel>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, bool>> expression, int labelWidth = 12, object htmlAttributes = null)
        {
            if (!expression.Body.NodeType.Equals(ExpressionType.MemberAccess))
            {
                return MvcHtmlString.Empty;
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var output = new StringBuilder();

            // Use the extended layout
            output.Append(@"<div class=""row form-group"">");

            output.Append(labelWidth == 12
                ? @"<div class=""col-sm-12"">"
                : $@"<div class=""offset-{labelWidth} col-sm-{12 - labelWidth}"">");

            output.Append(@"<div class=""form-check"">");
            output.Append(html.CheckBoxFor(expression, htmlAttributes ?? new { @class = "form-check-input" }));
            output.Append(html.LabelFor(expression, metadata.Description, new { @class = "form-check-label" }));

            output.Append(@"</div>");
            output.Append(@"</div>");
            output.Append(@"</div>");

            return MvcHtmlString.Create(output.ToString());
        }

        public static MvcHtmlString FormGroupPasswordHorizontalFor<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TResult>> expression, int labelWidth = 12, object htmlAttributes = null)
        {
            return FormGroupControlHorizontalFor(html, expression, HtmlHelperExtensionsControlTypes.Password, labelWidth, htmlAttributes);
        }

        public static MvcHtmlString FormGroupTextAreaHorizontalFor<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TResult>> expression, int labelWidth = 12, object htmlAttributes = null)
        {
            return FormGroupControlHorizontalFor(html, expression, HtmlHelperExtensionsControlTypes.TextArea, labelWidth, htmlAttributes);
        }

        public static MvcHtmlString FormGroupTextBoxHorizontalFor<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TResult>> expression, int labelWidth = 12, object htmlAttributes = null)
        {
            return FormGroupControlHorizontalFor(html, expression, HtmlHelperExtensionsControlTypes.TextBox, labelWidth, htmlAttributes);
        }

        public static MvcHtmlString MailLink<TModel>(this HtmlHelper<TModel> html, string emailAddress,
            IDictionary<string, object> htmlAttributes = null)
        {
            if (string.IsNullOrWhiteSpace(emailAddress) || !Validate.IsValidEmailAddress(emailAddress))
            {
                return MvcHtmlString.Empty;
            }

            var tag = new TagBuilder("a");

            tag.Attributes.Add("href", $"mailto:{emailAddress}");
            tag.MergeAttributes(htmlAttributes);
            tag.SetInnerText(emailAddress);

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        private static MvcHtmlString FormGroupControlHorizontalFor<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TResult>> expression, HtmlHelperExtensionsControlTypes controlType,
            int labelWidth = 12, object htmlAttributes = null)
        {
            if (!expression.Body.NodeType.Equals(ExpressionType.MemberAccess))
            {
                return MvcHtmlString.Empty;
            }

            if (!controlType.IsAny(HtmlHelperExtensionsControlTypes.Password,
                                   HtmlHelperExtensionsControlTypes.TextArea,
                                   HtmlHelperExtensionsControlTypes.TextBox))
            {
                return MvcHtmlString.Empty;
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var output = new StringBuilder();

            if (labelWidth == 12)
            {
                // Use the simple form-group Bootstrap layout
                output.Append(@"<div class=""form-group"">");
                output.Append(html.LabelFor(expression, metadata.Description));
            }
            else
            {
                // Use the extended layout
                output.Append(@"<div class=""row form-group"">");
                output.Append(html.LabelFor(expression, metadata.Description, new { @class = $"col-sm-{labelWidth} col-form-label" }));
                output.Append($@"<div class=""col-sm-{12 - labelWidth}"">");
            }

            switch (controlType)
            {
                case HtmlHelperExtensionsControlTypes.Password:
                    output.Append(html.PasswordFor(expression, htmlAttributes ?? new { @class = "form-control" }));
                    break;

                case HtmlHelperExtensionsControlTypes.TextArea:
                    output.Append(html.TextAreaFor(expression, htmlAttributes ?? new { @class = "form-control", placeholder = metadata.Watermark, rows = 5 }));
                    break;

                case HtmlHelperExtensionsControlTypes.TextBox:
                    output.Append(html.TextBoxFor(expression, htmlAttributes ?? new { @class = "form-control", placeholder = metadata.Watermark }));
                    break;
            }

            if (labelWidth != 12)
            {
                output.Append(@"</div>");
            }

            output.Append(@"</div>");

            return MvcHtmlString.Create(output.ToString());
        }
    }
}