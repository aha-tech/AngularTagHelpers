# AngularTagHelpers
Razor tag helpers to automatically include your compiled Angular files. Works both for development and production builds.

While the Angular-cli build chain generates an `index.html` file, that correctly references the generated JS and CSS files, it's often the case that you need to render some data from the server in a Razor Page or View.
When doing so, you need to be take into consideration that the cli generates different files based on whether you run `ng watch` or `ng build --prod`, and that the latter command adds hash values to the generated files.

To fix this, I've created these two tag helpers; one for CSS and one for JS. Just point them to the output folder of Angular-cli, and they will pick up the available files.

The tag helpers are similar to the `Microsoft.AspNetCore.Mvc.TagHelpers.ScriptTagHelper` except they order the generated `<script>` tags.

To use them, download the NuGet package (TODO), and in your razor page, import the tag helpers, like so:
```
@addTagHelper *, AhaTech.AngularTagHelpers
```

Then, add the `<angular-css-files src="...">` tag to the html header, and the  `<angular-js-files src="...">` tag to the body.

When running `ng watch`, no CSS files will be generated, and so, the `<angular-css-files>` tag will not output anything.

As I haven't figured out how to make a TagHelper that generates multiple, non-nested tags, the generated `<script>` tags are wrapped in a `<script-block>` tag, which shouldn't affect anything.

## Sample Index.cshtml
```html
@page
@model IndexModel
@addTagHelper *, AhaTech.AngularTagHelpers

<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>AngularTagHelpers</title>
  <base href="/">

  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="icon" type="image/x-icon" href="favicon.ico">
  
  <angular-css-files src="~/angular"></angular-css-files>

</head>
<body>
  
  <app-root></app-root>

  <angular-js-files src="~/angular"></angular-js-files>
  
</html>

```
