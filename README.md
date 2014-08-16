# CSSJanus

Converts CSS stylesheets between left-to-right and right-to-left. This is a C# port for .Net of [CSSJanus](https://code.google.com/p/cssjanus/), which is written in python.

## Basic usage
```cs
CssJanus cssJanus = new CssJanus();
string rtlCss = cssjanus.Transform( ltrCss );
```

## Advanced usage

``Transform( css, swapLtrRtlInUrl, swapLeftRightInUrl )``

* ``css`` (String) Stylesheet to transform
* ``swapLtrRtlInUrl`` (Boolean) Swap 'ltr' and 'rtl' in URLs
* ``swapLeftRightInUrl`` (Boolean) Swap 'left' and 'right' in URLs

### Preventing flipping
Use a ```/* @noflip */``` comment to protect a rule from being changed.

```css
.rule1 {
  /* Will be converted to margin-right */
  margin-left: 1em;
}
/* @noflip */
.rule2 {
  /* Will be preserved as margin-left */
  margin-left: 1em;
}
```

### Additional Resources
* [Demo video](http://google-opensource.blogspot.com/2008/03/cssjanus-helping-i18n-and-ltr-to-rtl.html)
* [Node.js version of CssJanus](https://github.com/cssjanus/cssjanus)
