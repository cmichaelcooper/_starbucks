
// place any jQuery/helper plugins in here, instead of separate, slower script files.
/*
	Breakpoints.js
	version 1.0
	
	Creates handy events for your responsive design breakpoints
	
	Copyright 2011 XOXCO, Inc
	http://xoxco.com/

	Documentation for this plugin lives here:
	http://xoxco.com/projects/code/breakpoints
	
	Licensed under the MIT license:
	http://www.opensource.org/licenses/mit-license.php

*/
(function ($) {

    var lastSize = 0;
    var interval = null;

    $.fn.resetBreakpoints = function () {
        $(window).unbind('resize');
        if (interval) {
            clearInterval(interval);
        }
        lastSize = 0;
    };

    $.fn.setBreakpoints = function (settings) {
        var options = jQuery.extend({
            distinct: true,
            breakpoints: new Array(320, 480, 768, 1024)
        }, settings);

        interval = setInterval(function () {

            var w = $(window).width();
            var done = false;

            for (var bp in options.breakpoints.sort(function (a, b) { return (b - a) })) {

                // fire onEnter when a browser expands into a new breakpoint
                // if in distinct mode, remove all other breakpoints first.
                if (!done && w >= options.breakpoints[bp] && lastSize < options.breakpoints[bp]) {
                    if (options.distinct) {
                        for (var x in options.breakpoints.sort(function (a, b) { return (b - a) })) {
                            if ($('body').hasClass('breakpoint-' + options.breakpoints[x])) {
                                $('body').removeClass('breakpoint-' + options.breakpoints[x]);
                                $(window).trigger('exitBreakpoint' + options.breakpoints[x]);
                            }
                        }
                        done = true;
                    }
                    $('body').addClass('breakpoint-' + options.breakpoints[bp]);
                    $(window).trigger('enterBreakpoint' + options.breakpoints[bp]);

                }

                // fire onExit when browser contracts out of a larger breakpoint
                if (w < options.breakpoints[bp] && lastSize >= options.breakpoints[bp]) {
                    $('body').removeClass('breakpoint-' + options.breakpoints[bp]);
                    $(window).trigger('exitBreakpoint' + options.breakpoints[bp]);

                }

                // if in distinct mode, fire onEnter when browser contracts into a smaller breakpoint
                if (
					options.distinct && // only one breakpoint at a time
					w >= options.breakpoints[bp] && // and we are in this one
					w < options.breakpoints[bp - 1] && // and smaller than the bigger one
					lastSize > w && // and we contracted
					lastSize > 0 &&  // and this is not the first time
					!$('body').hasClass('breakpoint-' + options.breakpoints[bp]) // and we aren't already in this breakpoint
					) {
                    $('body').addClass('breakpoint-' + options.breakpoints[bp]);
                    $(window).trigger('enterBreakpoint' + options.breakpoints[bp]);

                }
            }

            // set up for next call
            if (lastSize != w) {
                lastSize = w;
            }
        }, 250);
    };

})(jQuery);


/*
Copyright 2012 Igor Vaynberg

Version: 3.4.5 Timestamp: Mon Nov  4 08:22:42 PST 2013

This software is licensed under the Apache License, Version 2.0 (the "Apache License") or the GNU
General Public License version 2 (the "GPL License"). You may choose either license to govern your
use of this software only upon the condition that you accept all of the terms of either the Apache
License or the GPL License.

You may obtain a copy of the Apache License and the GPL License at:

http://www.apache.org/licenses/LICENSE-2.0
http://www.gnu.org/licenses/gpl-2.0.html

Unless required by applicable law or agreed to in writing, software distributed under the Apache License
or the GPL Licesnse is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the Apache License and the GPL License for the specific language governing
permissions and limitations under the Apache License and the GPL License.
*/
!function (a) { "undefined" == typeof a.fn.each2 && a.extend(a.fn, { each2: function (b) { for (var c = a([0]), d = -1, e = this.length; ++d < e && (c.context = c[0] = this[d]) && b.call(c[0], d, c) !== !1;); return this } }) }(jQuery), function (a, b) {
    "use strict"; function n(a) { var b, c, d, e; if (!a || a.length < 1) return a; for (b = "", c = 0, d = a.length; d > c; c++) e = a.charAt(c), b += m[e] || e; return b } function o(a, b) { for (var c = 0, d = b.length; d > c; c += 1) if (q(a, b[c])) return c; return -1 } function p() { var b = a(l); b.appendTo("body"); var c = { width: b.width() - b[0].clientWidth, height: b.height() - b[0].clientHeight }; return b.remove(), c } function q(a, c) { return a === c ? !0 : a === b || c === b ? !1 : null === a || null === c ? !1 : a.constructor === String ? a + "" == c + "" : c.constructor === String ? c + "" == a + "" : !1 } function r(b, c) { var d, e, f; if (null === b || b.length < 1) return []; for (d = b.split(c), e = 0, f = d.length; f > e; e += 1) d[e] = a.trim(d[e]); return d } function s(a) { return a.outerWidth(!1) - a.width() } function t(c) { var d = "keyup-change-value"; c.on("keydown", function () { a.data(c, d) === b && a.data(c, d, c.val()) }), c.on("keyup", function () { var e = a.data(c, d); e !== b && c.val() !== e && (a.removeData(c, d), c.trigger("keyup-change")) }) } function u(c) { c.on("mousemove", function (c) { var d = i; (d === b || d.x !== c.pageX || d.y !== c.pageY) && a(c.target).trigger("mousemove-filtered", c) }) } function v(a, c, d) { d = d || b; var e; return function () { var b = arguments; window.clearTimeout(e), e = window.setTimeout(function () { c.apply(d, b) }, a) } } function w(a) { var c, b = !1; return function () { return b === !1 && (c = a(), b = !0), c } } function x(a, b) { var c = v(a, function (a) { b.trigger("scroll-debounced", a) }); b.on("scroll", function (a) { o(a.target, b.get()) >= 0 && c(a) }) } function y(a) { a[0] !== document.activeElement && window.setTimeout(function () { var d, b = a[0], c = a.val().length; a.focus(), a.is(":visible") && b === document.activeElement && (b.setSelectionRange ? b.setSelectionRange(c, c) : b.createTextRange && (d = b.createTextRange(), d.collapse(!1), d.select())) }, 0) } function z(b) { b = a(b)[0]; var c = 0, d = 0; if ("selectionStart" in b) c = b.selectionStart, d = b.selectionEnd - c; else if ("selection" in document) { b.focus(); var e = document.selection.createRange(); d = document.selection.createRange().text.length, e.moveStart("character", -b.value.length), c = e.text.length - d } return { offset: c, length: d } } function A(a) { a.preventDefault(), a.stopPropagation() } function B(a) { a.preventDefault(), a.stopImmediatePropagation() } function C(b) { if (!h) { var c = b[0].currentStyle || window.getComputedStyle(b[0], null); h = a(document.createElement("div")).css({ position: "absolute", left: "-10000px", top: "-10000px", display: "none", fontSize: c.fontSize, fontFamily: c.fontFamily, fontStyle: c.fontStyle, fontWeight: c.fontWeight, letterSpacing: c.letterSpacing, textTransform: c.textTransform, whiteSpace: "nowrap" }), h.attr("class", "select2-sizer"), a("body").append(h) } return h.text(b.val()), h.width() } function D(b, c, d) { var e, g, f = []; e = b.attr("class"), e && (e = "" + e, a(e.split(" ")).each2(function () { 0 === this.indexOf("select2-") && f.push(this) })), e = c.attr("class"), e && (e = "" + e, a(e.split(" ")).each2(function () { 0 !== this.indexOf("select2-") && (g = d(this), g && f.push(g)) })), b.attr("class", f.join(" ")) } function E(a, b, c, d) { var e = n(a.toUpperCase()).indexOf(n(b.toUpperCase())), f = b.length; return 0 > e ? (c.push(d(a)), void 0) : (c.push(d(a.substring(0, e))), c.push("<span class='select2-match'>"), c.push(d(a.substring(e, e + f))), c.push("</span>"), c.push(d(a.substring(e + f, a.length))), void 0) } function F(a) { var b = { "\\": "&#92;", "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;", "/": "&#47;" }; return String(a).replace(/[&<>"'\/\\]/g, function (a) { return b[a] }) } function G(c) { var d, e = null, f = c.quietMillis || 100, g = c.url, h = this; return function (i) { window.clearTimeout(d), d = window.setTimeout(function () { var d = c.data, f = g, j = c.transport || a.fn.select2.ajaxDefaults.transport, k = { type: c.type || "GET", cache: c.cache || !1, jsonpCallback: c.jsonpCallback || b, dataType: c.dataType || "json" }, l = a.extend({}, a.fn.select2.ajaxDefaults.params, k); d = d ? d.call(h, i.term, i.page, i.context) : null, f = "function" == typeof f ? f.call(h, i.term, i.page, i.context) : f, e && e.abort(), c.params && (a.isFunction(c.params) ? a.extend(l, c.params.call(h)) : a.extend(l, c.params)), a.extend(l, { url: f, dataType: c.dataType, data: d, success: function (a) { var b = c.results(a, i.page); i.callback(b) } }), e = j.call(h, l) }, f) } } function H(b) { var d, e, c = b, f = function (a) { return "" + a.text }; a.isArray(c) && (e = c, c = { results: e }), a.isFunction(c) === !1 && (e = c, c = function () { return e }); var g = c(); return g.text && (f = g.text, a.isFunction(f) || (d = g.text, f = function (a) { return a[d] })), function (b) { var g, d = b.term, e = { results: [] }; return "" === d ? (b.callback(c()), void 0) : (g = function (c, e) { var h, i; if (c = c[0], c.children) { h = {}; for (i in c) c.hasOwnProperty(i) && (h[i] = c[i]); h.children = [], a(c.children).each2(function (a, b) { g(b, h.children) }), (h.children.length || b.matcher(d, f(h), c)) && e.push(h) } else b.matcher(d, f(c), c) && e.push(c) }, a(c().results).each2(function (a, b) { g(b, e.results) }), b.callback(e), void 0) } } function I(c) { var d = a.isFunction(c); return function (e) { var f = e.term, g = { results: [] }; a(d ? c() : c).each(function () { var a = this.text !== b, c = a ? this.text : this; ("" === f || e.matcher(f, c)) && g.results.push(a ? this : { id: this, text: this }) }), e.callback(g) } } function J(b, c) { if (a.isFunction(b)) return !0; if (!b) return !1; throw new Error(c + " must be a function or a falsy value") } function K(b) { return a.isFunction(b) ? b() : b } function L(b) { var c = 0; return a.each(b, function (a, b) { b.children ? c += L(b.children) : c++ }), c } function M(a, c, d, e) { var h, i, j, k, l, f = a, g = !1; if (!e.createSearchChoice || !e.tokenSeparators || e.tokenSeparators.length < 1) return b; for (; ;) { for (i = -1, j = 0, k = e.tokenSeparators.length; k > j && (l = e.tokenSeparators[j], i = a.indexOf(l), !(i >= 0)) ; j++); if (0 > i) break; if (h = a.substring(0, i), a = a.substring(i + l.length), h.length > 0 && (h = e.createSearchChoice.call(this, h, c), h !== b && null !== h && e.id(h) !== b && null !== e.id(h))) { for (g = !1, j = 0, k = c.length; k > j; j++) if (q(e.id(h), e.id(c[j]))) { g = !0; break } g || d(h) } } return f !== a ? a : void 0 } function N(b, c) { var d = function () { }; return d.prototype = new b, d.prototype.constructor = d, d.prototype.parent = b.prototype, d.prototype = a.extend(d.prototype, c), d } if (window.Select2 === b) {
        var c, d, e, f, g, h, j, k, i = { x: 0, y: 0 }, c = { TAB: 9, ENTER: 13, ESC: 27, SPACE: 32, LEFT: 37, UP: 38, RIGHT: 39, DOWN: 40, SHIFT: 16, CTRL: 17, ALT: 18, PAGE_UP: 33, PAGE_DOWN: 34, HOME: 36, END: 35, BACKSPACE: 8, DELETE: 46, isArrow: function (a) { switch (a = a.which ? a.which : a) { case c.LEFT: case c.RIGHT: case c.UP: case c.DOWN: return !0 } return !1 }, isControl: function (a) { var b = a.which; switch (b) { case c.SHIFT: case c.CTRL: case c.ALT: return !0 } return a.metaKey ? !0 : !1 }, isFunctionKey: function (a) { return a = a.which ? a.which : a, a >= 112 && 123 >= a } }, l = "<div class='select2-measure-scrollbar'></div>", m = { "\u24b6": "A", "\uff21": "A", "\xc0": "A", "\xc1": "A", "\xc2": "A", "\u1ea6": "A", "\u1ea4": "A", "\u1eaa": "A", "\u1ea8": "A", "\xc3": "A", "\u0100": "A", "\u0102": "A", "\u1eb0": "A", "\u1eae": "A", "\u1eb4": "A", "\u1eb2": "A", "\u0226": "A", "\u01e0": "A", "\xc4": "A", "\u01de": "A", "\u1ea2": "A", "\xc5": "A", "\u01fa": "A", "\u01cd": "A", "\u0200": "A", "\u0202": "A", "\u1ea0": "A", "\u1eac": "A", "\u1eb6": "A", "\u1e00": "A", "\u0104": "A", "\u023a": "A", "\u2c6f": "A", "\ua732": "AA", "\xc6": "AE", "\u01fc": "AE", "\u01e2": "AE", "\ua734": "AO", "\ua736": "AU", "\ua738": "AV", "\ua73a": "AV", "\ua73c": "AY", "\u24b7": "B", "\uff22": "B", "\u1e02": "B", "\u1e04": "B", "\u1e06": "B", "\u0243": "B", "\u0182": "B", "\u0181": "B", "\u24b8": "C", "\uff23": "C", "\u0106": "C", "\u0108": "C", "\u010a": "C", "\u010c": "C", "\xc7": "C", "\u1e08": "C", "\u0187": "C", "\u023b": "C", "\ua73e": "C", "\u24b9": "D", "\uff24": "D", "\u1e0a": "D", "\u010e": "D", "\u1e0c": "D", "\u1e10": "D", "\u1e12": "D", "\u1e0e": "D", "\u0110": "D", "\u018b": "D", "\u018a": "D", "\u0189": "D", "\ua779": "D", "\u01f1": "DZ", "\u01c4": "DZ", "\u01f2": "Dz", "\u01c5": "Dz", "\u24ba": "E", "\uff25": "E", "\xc8": "E", "\xc9": "E", "\xca": "E", "\u1ec0": "E", "\u1ebe": "E", "\u1ec4": "E", "\u1ec2": "E", "\u1ebc": "E", "\u0112": "E", "\u1e14": "E", "\u1e16": "E", "\u0114": "E", "\u0116": "E", "\xcb": "E", "\u1eba": "E", "\u011a": "E", "\u0204": "E", "\u0206": "E", "\u1eb8": "E", "\u1ec6": "E", "\u0228": "E", "\u1e1c": "E", "\u0118": "E", "\u1e18": "E", "\u1e1a": "E", "\u0190": "E", "\u018e": "E", "\u24bb": "F", "\uff26": "F", "\u1e1e": "F", "\u0191": "F", "\ua77b": "F", "\u24bc": "G", "\uff27": "G", "\u01f4": "G", "\u011c": "G", "\u1e20": "G", "\u011e": "G", "\u0120": "G", "\u01e6": "G", "\u0122": "G", "\u01e4": "G", "\u0193": "G", "\ua7a0": "G", "\ua77d": "G", "\ua77e": "G", "\u24bd": "H", "\uff28": "H", "\u0124": "H", "\u1e22": "H", "\u1e26": "H", "\u021e": "H", "\u1e24": "H", "\u1e28": "H", "\u1e2a": "H", "\u0126": "H", "\u2c67": "H", "\u2c75": "H", "\ua78d": "H", "\u24be": "I", "\uff29": "I", "\xcc": "I", "\xcd": "I", "\xce": "I", "\u0128": "I", "\u012a": "I", "\u012c": "I", "\u0130": "I", "\xcf": "I", "\u1e2e": "I", "\u1ec8": "I", "\u01cf": "I", "\u0208": "I", "\u020a": "I", "\u1eca": "I", "\u012e": "I", "\u1e2c": "I", "\u0197": "I", "\u24bf": "J", "\uff2a": "J", "\u0134": "J", "\u0248": "J", "\u24c0": "K", "\uff2b": "K", "\u1e30": "K", "\u01e8": "K", "\u1e32": "K", "\u0136": "K", "\u1e34": "K", "\u0198": "K", "\u2c69": "K", "\ua740": "K", "\ua742": "K", "\ua744": "K", "\ua7a2": "K", "\u24c1": "L", "\uff2c": "L", "\u013f": "L", "\u0139": "L", "\u013d": "L", "\u1e36": "L", "\u1e38": "L", "\u013b": "L", "\u1e3c": "L", "\u1e3a": "L", "\u0141": "L", "\u023d": "L", "\u2c62": "L", "\u2c60": "L", "\ua748": "L", "\ua746": "L", "\ua780": "L", "\u01c7": "LJ", "\u01c8": "Lj", "\u24c2": "M", "\uff2d": "M", "\u1e3e": "M", "\u1e40": "M", "\u1e42": "M", "\u2c6e": "M", "\u019c": "M", "\u24c3": "N", "\uff2e": "N", "\u01f8": "N", "\u0143": "N", "\xd1": "N", "\u1e44": "N", "\u0147": "N", "\u1e46": "N", "\u0145": "N", "\u1e4a": "N", "\u1e48": "N", "\u0220": "N", "\u019d": "N", "\ua790": "N", "\ua7a4": "N", "\u01ca": "NJ", "\u01cb": "Nj", "\u24c4": "O", "\uff2f": "O", "\xd2": "O", "\xd3": "O", "\xd4": "O", "\u1ed2": "O", "\u1ed0": "O", "\u1ed6": "O", "\u1ed4": "O", "\xd5": "O", "\u1e4c": "O", "\u022c": "O", "\u1e4e": "O", "\u014c": "O", "\u1e50": "O", "\u1e52": "O", "\u014e": "O", "\u022e": "O", "\u0230": "O", "\xd6": "O", "\u022a": "O", "\u1ece": "O", "\u0150": "O", "\u01d1": "O", "\u020c": "O", "\u020e": "O", "\u01a0": "O", "\u1edc": "O", "\u1eda": "O", "\u1ee0": "O", "\u1ede": "O", "\u1ee2": "O", "\u1ecc": "O", "\u1ed8": "O", "\u01ea": "O", "\u01ec": "O", "\xd8": "O", "\u01fe": "O", "\u0186": "O", "\u019f": "O", "\ua74a": "O", "\ua74c": "O", "\u01a2": "OI", "\ua74e": "OO", "\u0222": "OU", "\u24c5": "P", "\uff30": "P", "\u1e54": "P", "\u1e56": "P", "\u01a4": "P", "\u2c63": "P", "\ua750": "P", "\ua752": "P", "\ua754": "P", "\u24c6": "Q", "\uff31": "Q", "\ua756": "Q", "\ua758": "Q", "\u024a": "Q", "\u24c7": "R", "\uff32": "R", "\u0154": "R", "\u1e58": "R", "\u0158": "R", "\u0210": "R", "\u0212": "R", "\u1e5a": "R", "\u1e5c": "R", "\u0156": "R", "\u1e5e": "R", "\u024c": "R", "\u2c64": "R", "\ua75a": "R", "\ua7a6": "R", "\ua782": "R", "\u24c8": "S", "\uff33": "S", "\u1e9e": "S", "\u015a": "S", "\u1e64": "S", "\u015c": "S", "\u1e60": "S", "\u0160": "S", "\u1e66": "S", "\u1e62": "S", "\u1e68": "S", "\u0218": "S", "\u015e": "S", "\u2c7e": "S", "\ua7a8": "S", "\ua784": "S", "\u24c9": "T", "\uff34": "T", "\u1e6a": "T", "\u0164": "T", "\u1e6c": "T", "\u021a": "T", "\u0162": "T", "\u1e70": "T", "\u1e6e": "T", "\u0166": "T", "\u01ac": "T", "\u01ae": "T", "\u023e": "T", "\ua786": "T", "\ua728": "TZ", "\u24ca": "U", "\uff35": "U", "\xd9": "U", "\xda": "U", "\xdb": "U", "\u0168": "U", "\u1e78": "U", "\u016a": "U", "\u1e7a": "U", "\u016c": "U", "\xdc": "U", "\u01db": "U", "\u01d7": "U", "\u01d5": "U", "\u01d9": "U", "\u1ee6": "U", "\u016e": "U", "\u0170": "U", "\u01d3": "U", "\u0214": "U", "\u0216": "U", "\u01af": "U", "\u1eea": "U", "\u1ee8": "U", "\u1eee": "U", "\u1eec": "U", "\u1ef0": "U", "\u1ee4": "U", "\u1e72": "U", "\u0172": "U", "\u1e76": "U", "\u1e74": "U", "\u0244": "U", "\u24cb": "V", "\uff36": "V", "\u1e7c": "V", "\u1e7e": "V", "\u01b2": "V", "\ua75e": "V", "\u0245": "V", "\ua760": "VY", "\u24cc": "W", "\uff37": "W", "\u1e80": "W", "\u1e82": "W", "\u0174": "W", "\u1e86": "W", "\u1e84": "W", "\u1e88": "W", "\u2c72": "W", "\u24cd": "X", "\uff38": "X", "\u1e8a": "X", "\u1e8c": "X", "\u24ce": "Y", "\uff39": "Y", "\u1ef2": "Y", "\xdd": "Y", "\u0176": "Y", "\u1ef8": "Y", "\u0232": "Y", "\u1e8e": "Y", "\u0178": "Y", "\u1ef6": "Y", "\u1ef4": "Y", "\u01b3": "Y", "\u024e": "Y", "\u1efe": "Y", "\u24cf": "Z", "\uff3a": "Z", "\u0179": "Z", "\u1e90": "Z", "\u017b": "Z", "\u017d": "Z", "\u1e92": "Z", "\u1e94": "Z", "\u01b5": "Z", "\u0224": "Z", "\u2c7f": "Z", "\u2c6b": "Z", "\ua762": "Z", "\u24d0": "a", "\uff41": "a", "\u1e9a": "a", "\xe0": "a", "\xe1": "a", "\xe2": "a", "\u1ea7": "a", "\u1ea5": "a", "\u1eab": "a", "\u1ea9": "a", "\xe3": "a", "\u0101": "a", "\u0103": "a", "\u1eb1": "a", "\u1eaf": "a", "\u1eb5": "a", "\u1eb3": "a", "\u0227": "a", "\u01e1": "a", "\xe4": "a", "\u01df": "a", "\u1ea3": "a", "\xe5": "a", "\u01fb": "a", "\u01ce": "a", "\u0201": "a", "\u0203": "a", "\u1ea1": "a", "\u1ead": "a", "\u1eb7": "a", "\u1e01": "a", "\u0105": "a", "\u2c65": "a", "\u0250": "a", "\ua733": "aa", "\xe6": "ae", "\u01fd": "ae", "\u01e3": "ae", "\ua735": "ao", "\ua737": "au", "\ua739": "av", "\ua73b": "av", "\ua73d": "ay", "\u24d1": "b", "\uff42": "b", "\u1e03": "b", "\u1e05": "b", "\u1e07": "b", "\u0180": "b", "\u0183": "b", "\u0253": "b", "\u24d2": "c", "\uff43": "c", "\u0107": "c", "\u0109": "c", "\u010b": "c", "\u010d": "c", "\xe7": "c", "\u1e09": "c", "\u0188": "c", "\u023c": "c", "\ua73f": "c", "\u2184": "c", "\u24d3": "d", "\uff44": "d", "\u1e0b": "d", "\u010f": "d", "\u1e0d": "d", "\u1e11": "d", "\u1e13": "d", "\u1e0f": "d", "\u0111": "d", "\u018c": "d", "\u0256": "d", "\u0257": "d", "\ua77a": "d", "\u01f3": "dz", "\u01c6": "dz", "\u24d4": "e", "\uff45": "e", "\xe8": "e", "\xe9": "e", "\xea": "e", "\u1ec1": "e", "\u1ebf": "e", "\u1ec5": "e", "\u1ec3": "e", "\u1ebd": "e", "\u0113": "e", "\u1e15": "e", "\u1e17": "e", "\u0115": "e", "\u0117": "e", "\xeb": "e", "\u1ebb": "e", "\u011b": "e", "\u0205": "e", "\u0207": "e", "\u1eb9": "e", "\u1ec7": "e", "\u0229": "e", "\u1e1d": "e", "\u0119": "e", "\u1e19": "e", "\u1e1b": "e", "\u0247": "e", "\u025b": "e", "\u01dd": "e", "\u24d5": "f", "\uff46": "f", "\u1e1f": "f", "\u0192": "f", "\ua77c": "f", "\u24d6": "g", "\uff47": "g", "\u01f5": "g", "\u011d": "g", "\u1e21": "g", "\u011f": "g", "\u0121": "g", "\u01e7": "g", "\u0123": "g", "\u01e5": "g", "\u0260": "g", "\ua7a1": "g", "\u1d79": "g", "\ua77f": "g", "\u24d7": "h", "\uff48": "h", "\u0125": "h", "\u1e23": "h", "\u1e27": "h", "\u021f": "h", "\u1e25": "h", "\u1e29": "h", "\u1e2b": "h", "\u1e96": "h", "\u0127": "h", "\u2c68": "h", "\u2c76": "h", "\u0265": "h", "\u0195": "hv", "\u24d8": "i", "\uff49": "i", "\xec": "i", "\xed": "i", "\xee": "i", "\u0129": "i", "\u012b": "i", "\u012d": "i", "\xef": "i", "\u1e2f": "i", "\u1ec9": "i", "\u01d0": "i", "\u0209": "i", "\u020b": "i", "\u1ecb": "i", "\u012f": "i", "\u1e2d": "i", "\u0268": "i", "\u0131": "i", "\u24d9": "j", "\uff4a": "j", "\u0135": "j", "\u01f0": "j", "\u0249": "j", "\u24da": "k", "\uff4b": "k", "\u1e31": "k", "\u01e9": "k", "\u1e33": "k", "\u0137": "k", "\u1e35": "k", "\u0199": "k", "\u2c6a": "k", "\ua741": "k", "\ua743": "k", "\ua745": "k", "\ua7a3": "k", "\u24db": "l", "\uff4c": "l", "\u0140": "l", "\u013a": "l", "\u013e": "l", "\u1e37": "l", "\u1e39": "l", "\u013c": "l", "\u1e3d": "l", "\u1e3b": "l", "\u017f": "l", "\u0142": "l", "\u019a": "l", "\u026b": "l", "\u2c61": "l", "\ua749": "l", "\ua781": "l", "\ua747": "l", "\u01c9": "lj", "\u24dc": "m", "\uff4d": "m", "\u1e3f": "m", "\u1e41": "m", "\u1e43": "m", "\u0271": "m", "\u026f": "m", "\u24dd": "n", "\uff4e": "n", "\u01f9": "n", "\u0144": "n", "\xf1": "n", "\u1e45": "n", "\u0148": "n", "\u1e47": "n", "\u0146": "n", "\u1e4b": "n", "\u1e49": "n", "\u019e": "n", "\u0272": "n", "\u0149": "n", "\ua791": "n", "\ua7a5": "n", "\u01cc": "nj", "\u24de": "o", "\uff4f": "o", "\xf2": "o", "\xf3": "o", "\xf4": "o", "\u1ed3": "o", "\u1ed1": "o", "\u1ed7": "o", "\u1ed5": "o", "\xf5": "o", "\u1e4d": "o", "\u022d": "o", "\u1e4f": "o", "\u014d": "o", "\u1e51": "o", "\u1e53": "o", "\u014f": "o", "\u022f": "o", "\u0231": "o", "\xf6": "o", "\u022b": "o", "\u1ecf": "o", "\u0151": "o", "\u01d2": "o", "\u020d": "o", "\u020f": "o", "\u01a1": "o", "\u1edd": "o", "\u1edb": "o", "\u1ee1": "o", "\u1edf": "o", "\u1ee3": "o", "\u1ecd": "o", "\u1ed9": "o", "\u01eb": "o", "\u01ed": "o", "\xf8": "o", "\u01ff": "o", "\u0254": "o", "\ua74b": "o", "\ua74d": "o", "\u0275": "o", "\u01a3": "oi", "\u0223": "ou", "\ua74f": "oo", "\u24df": "p", "\uff50": "p", "\u1e55": "p", "\u1e57": "p", "\u01a5": "p", "\u1d7d": "p", "\ua751": "p", "\ua753": "p", "\ua755": "p", "\u24e0": "q", "\uff51": "q", "\u024b": "q", "\ua757": "q", "\ua759": "q", "\u24e1": "r", "\uff52": "r", "\u0155": "r", "\u1e59": "r", "\u0159": "r", "\u0211": "r", "\u0213": "r", "\u1e5b": "r", "\u1e5d": "r", "\u0157": "r", "\u1e5f": "r", "\u024d": "r", "\u027d": "r", "\ua75b": "r", "\ua7a7": "r", "\ua783": "r", "\u24e2": "s", "\uff53": "s", "\xdf": "s", "\u015b": "s", "\u1e65": "s", "\u015d": "s", "\u1e61": "s", "\u0161": "s", "\u1e67": "s", "\u1e63": "s", "\u1e69": "s", "\u0219": "s", "\u015f": "s", "\u023f": "s", "\ua7a9": "s", "\ua785": "s", "\u1e9b": "s", "\u24e3": "t", "\uff54": "t", "\u1e6b": "t", "\u1e97": "t", "\u0165": "t", "\u1e6d": "t", "\u021b": "t", "\u0163": "t", "\u1e71": "t", "\u1e6f": "t", "\u0167": "t", "\u01ad": "t", "\u0288": "t", "\u2c66": "t", "\ua787": "t", "\ua729": "tz", "\u24e4": "u", "\uff55": "u", "\xf9": "u", "\xfa": "u", "\xfb": "u", "\u0169": "u", "\u1e79": "u", "\u016b": "u", "\u1e7b": "u", "\u016d": "u", "\xfc": "u", "\u01dc": "u", "\u01d8": "u", "\u01d6": "u", "\u01da": "u", "\u1ee7": "u", "\u016f": "u", "\u0171": "u", "\u01d4": "u", "\u0215": "u", "\u0217": "u", "\u01b0": "u", "\u1eeb": "u", "\u1ee9": "u", "\u1eef": "u", "\u1eed": "u", "\u1ef1": "u", "\u1ee5": "u", "\u1e73": "u", "\u0173": "u", "\u1e77": "u", "\u1e75": "u", "\u0289": "u", "\u24e5": "v", "\uff56": "v", "\u1e7d": "v", "\u1e7f": "v", "\u028b": "v", "\ua75f": "v", "\u028c": "v", "\ua761": "vy", "\u24e6": "w", "\uff57": "w", "\u1e81": "w", "\u1e83": "w", "\u0175": "w", "\u1e87": "w", "\u1e85": "w", "\u1e98": "w", "\u1e89": "w", "\u2c73": "w", "\u24e7": "x", "\uff58": "x", "\u1e8b": "x", "\u1e8d": "x", "\u24e8": "y", "\uff59": "y", "\u1ef3": "y", "\xfd": "y", "\u0177": "y", "\u1ef9": "y", "\u0233": "y", "\u1e8f": "y", "\xff": "y", "\u1ef7": "y", "\u1e99": "y", "\u1ef5": "y", "\u01b4": "y", "\u024f": "y", "\u1eff": "y", "\u24e9": "z", "\uff5a": "z", "\u017a": "z", "\u1e91": "z", "\u017c": "z", "\u017e": "z", "\u1e93": "z", "\u1e95": "z", "\u01b6": "z", "\u0225": "z", "\u0240": "z", "\u2c6c": "z", "\ua763": "z" }; j = a(document), g = function () { var a = 1; return function () { return a++ } }(), j.on("mousemove", function (a) { i.x = a.pageX, i.y = a.pageY }), d = N(Object, {
            bind: function (a) { var b = this; return function () { a.apply(b, arguments) } }, init: function (c) { var d, e, f = ".select2-results"; this.opts = c = this.prepareOpts(c), this.id = c.id, c.element.data("select2") !== b && null !== c.element.data("select2") && c.element.data("select2").destroy(), this.container = this.createContainer(), this.containerId = "s2id_" + (c.element.attr("id") || "autogen" + g()), this.containerSelector = "#" + this.containerId.replace(/([;&,\.\+\*\~':"\!\^#$%@\[\]\(\)=>\|])/g, "\\$1"), this.container.attr("id", this.containerId), this.body = w(function () { return c.element.closest("body") }), D(this.container, this.opts.element, this.opts.adaptContainerCssClass), this.container.attr("style", c.element.attr("style")), this.container.css(K(c.containerCss)), this.container.addClass(K(c.containerCssClass)), this.elementTabIndex = this.opts.element.attr("tabindex"), this.opts.element.data("select2", this).attr("tabindex", "-1").before(this.container).on("click.select2", A), this.container.data("select2", this), this.dropdown = this.container.find(".select2-drop"), D(this.dropdown, this.opts.element, this.opts.adaptDropdownCssClass), this.dropdown.addClass(K(c.dropdownCssClass)), this.dropdown.data("select2", this), this.dropdown.on("click", A), this.results = d = this.container.find(f), this.search = e = this.container.find("input.select2-input"), this.queryCount = 0, this.resultsPage = 0, this.context = null, this.initContainer(), this.container.on("click", A), u(this.results), this.dropdown.on("mousemove-filtered touchstart touchmove touchend", f, this.bind(this.highlightUnderEvent)), x(80, this.results), this.dropdown.on("scroll-debounced", f, this.bind(this.loadMoreIfNeeded)), a(this.container).on("change", ".select2-input", function (a) { a.stopPropagation() }), a(this.dropdown).on("change", ".select2-input", function (a) { a.stopPropagation() }), a.fn.mousewheel && d.mousewheel(function (a, b, c, e) { var f = d.scrollTop(); e > 0 && 0 >= f - e ? (d.scrollTop(0), A(a)) : 0 > e && d.get(0).scrollHeight - d.scrollTop() + e <= d.height() && (d.scrollTop(d.get(0).scrollHeight - d.height()), A(a)) }), t(e), e.on("keyup-change input paste", this.bind(this.updateResults)), e.on("focus", function () { e.addClass("select2-focused") }), e.on("blur", function () { e.removeClass("select2-focused") }), this.dropdown.on("mouseup", f, this.bind(function (b) { a(b.target).closest(".select2-result-selectable").length > 0 && (this.highlightUnderEvent(b), this.selectHighlighted(b)) })), this.dropdown.on("click mouseup mousedown", function (a) { a.stopPropagation() }), a.isFunction(this.opts.initSelection) && (this.initSelection(), this.monitorSource()), null !== c.maximumInputLength && this.search.attr("maxlength", c.maximumInputLength); var h = c.element.prop("disabled"); h === b && (h = !1), this.enable(!h); var i = c.element.prop("readonly"); i === b && (i = !1), this.readonly(i), k = k || p(), this.autofocus = c.element.prop("autofocus"), c.element.prop("autofocus", !1), this.autofocus && this.focus(), this.nextSearchTerm = b }, destroy: function () { var a = this.opts.element, c = a.data("select2"); this.close(), this.propertyObserver && (delete this.propertyObserver, this.propertyObserver = null), c !== b && (c.container.remove(), c.dropdown.remove(), a.removeClass("select2-offscreen").removeData("select2").off(".select2").prop("autofocus", this.autofocus || !1), this.elementTabIndex ? a.attr({ tabindex: this.elementTabIndex }) : a.removeAttr("tabindex"), a.show()) }, optionToData: function (a) { return a.is("option") ? { id: a.prop("value"), text: a.text(), element: a.get(), css: a.attr("class"), disabled: a.prop("disabled"), locked: q(a.attr("locked"), "locked") || q(a.data("locked"), !0) } : a.is("optgroup") ? { text: a.attr("label"), children: [], element: a.get(), css: a.attr("class") } : void 0 }, prepareOpts: function (c) { var d, e, f, g, h = this; if (d = c.element, "select" === d.get(0).tagName.toLowerCase() && (this.select = e = c.element), e && a.each(["id", "multiple", "ajax", "query", "createSearchChoice", "initSelection", "data", "tags"], function () { if (this in c) throw new Error("Option '" + this + "' is not allowed for Select2 when attached to a <select> element.") }), c = a.extend({}, { populateResults: function (d, e, f) { var g, i = this.opts.id; g = function (d, e, j) { var k, l, m, n, o, p, q, r, s, t; for (d = c.sortResults(d, e, f), k = 0, l = d.length; l > k; k += 1) m = d[k], o = m.disabled === !0, n = !o && i(m) !== b, p = m.children && m.children.length > 0, q = a("<li></li>"), q.addClass("select2-results-dept-" + j), q.addClass("select2-result"), q.addClass(n ? "select2-result-selectable" : "select2-result-unselectable"), o && q.addClass("select2-disabled"), p && q.addClass("select2-result-with-children"), q.addClass(h.opts.formatResultCssClass(m)), r = a(document.createElement("div")), r.addClass("select2-result-label"), t = c.formatResult(m, r, f, h.opts.escapeMarkup), t !== b && r.html(t), q.append(r), p && (s = a("<ul></ul>"), s.addClass("select2-result-sub"), g(m.children, s, j + 1), q.append(s)), q.data("select2-data", m), e.append(q) }, g(e, d, 0) } }, a.fn.select2.defaults, c), "function" != typeof c.id && (f = c.id, c.id = function (a) { return a[f] }), a.isArray(c.element.data("select2Tags"))) { if ("tags" in c) throw "tags specified as both an attribute 'data-select2-tags' and in options of Select2 " + c.element.attr("id"); c.tags = c.element.data("select2Tags") } if (e ? (c.query = this.bind(function (a) { var f, g, i, c = { results: [], more: !1 }, e = a.term; i = function (b, c) { var d; b.is("option") ? a.matcher(e, b.text(), b) && c.push(h.optionToData(b)) : b.is("optgroup") && (d = h.optionToData(b), b.children().each2(function (a, b) { i(b, d.children) }), d.children.length > 0 && c.push(d)) }, f = d.children(), this.getPlaceholder() !== b && f.length > 0 && (g = this.getPlaceholderOption(), g && (f = f.not(g))), f.each2(function (a, b) { i(b, c.results) }), a.callback(c) }), c.id = function (a) { return a.id }, c.formatResultCssClass = function (a) { return a.css }) : "query" in c || ("ajax" in c ? (g = c.element.data("ajax-url"), g && g.length > 0 && (c.ajax.url = g), c.query = G.call(c.element, c.ajax)) : "data" in c ? c.query = H(c.data) : "tags" in c && (c.query = I(c.tags), c.createSearchChoice === b && (c.createSearchChoice = function (b) { return { id: a.trim(b), text: a.trim(b) } }), c.initSelection === b && (c.initSelection = function (b, d) { var e = []; a(r(b.val(), c.separator)).each(function () { var b = { id: this, text: this }, d = c.tags; a.isFunction(d) && (d = d()), a(d).each(function () { return q(this.id, b.id) ? (b = this, !1) : void 0 }), e.push(b) }), d(e) }))), "function" != typeof c.query) throw "query function not defined for Select2 " + c.element.attr("id"); return c }, monitorSource: function () { var c, d, a = this.opts.element; a.on("change.select2", this.bind(function () { this.opts.element.data("select2-change-triggered") !== !0 && this.initSelection() })), c = this.bind(function () { var c = a.prop("disabled"); c === b && (c = !1), this.enable(!c); var d = a.prop("readonly"); d === b && (d = !1), this.readonly(d), D(this.container, this.opts.element, this.opts.adaptContainerCssClass), this.container.addClass(K(this.opts.containerCssClass)), D(this.dropdown, this.opts.element, this.opts.adaptDropdownCssClass), this.dropdown.addClass(K(this.opts.dropdownCssClass)) }), a.on("propertychange.select2", c), this.mutationCallback === b && (this.mutationCallback = function (a) { a.forEach(c) }), d = window.MutationObserver || window.WebKitMutationObserver || window.MozMutationObserver, d !== b && (this.propertyObserver && (delete this.propertyObserver, this.propertyObserver = null), this.propertyObserver = new d(this.mutationCallback), this.propertyObserver.observe(a.get(0), { attributes: !0, subtree: !1 })) }, triggerSelect: function (b) { var c = a.Event("select2-selecting", { val: this.id(b), object: b }); return this.opts.element.trigger(c), !c.isDefaultPrevented() }, triggerChange: function (b) { b = b || {}, b = a.extend({}, b, { type: "change", val: this.val() }), this.opts.element.data("select2-change-triggered", !0), this.opts.element.trigger(b), this.opts.element.data("select2-change-triggered", !1), this.opts.element.click(), this.opts.blurOnChange && this.opts.element.blur() }, isInterfaceEnabled: function () { return this.enabledInterface === !0 }, enableInterface: function () { var a = this._enabled && !this._readonly, b = !a; return a === this.enabledInterface ? !1 : (this.container.toggleClass("select2-container-disabled", b), this.close(), this.enabledInterface = a, !0) }, enable: function (a) { a === b && (a = !0), this._enabled !== a && (this._enabled = a, this.opts.element.prop("disabled", !a), this.enableInterface()) }, disable: function () { this.enable(!1) }, readonly: function (a) { return a === b && (a = !1), this._readonly === a ? !1 : (this._readonly = a, this.opts.element.prop("readonly", a), this.enableInterface(), !0) }, opened: function () { return this.container.hasClass("select2-dropdown-open") }, positionDropdown: function () { var t, u, v, w, x, b = this.dropdown, c = this.container.offset(), d = this.container.outerHeight(!1), e = this.container.outerWidth(!1), f = b.outerHeight(!1), g = a(window), h = g.width(), i = g.height(), j = g.scrollLeft() + h, l = g.scrollTop() + i, m = c.top + d, n = c.left, o = l >= m + f, p = c.top - f >= this.body().scrollTop(), q = b.outerWidth(!1), r = j >= n + q, s = b.hasClass("select2-drop-above"); s ? (u = !0, !p && o && (v = !0, u = !1)) : (u = !1, !o && p && (v = !0, u = !0)), v && (b.hide(), c = this.container.offset(), d = this.container.outerHeight(!1), e = this.container.outerWidth(!1), f = b.outerHeight(!1), j = g.scrollLeft() + h, l = g.scrollTop() + i, m = c.top + d, n = c.left, q = b.outerWidth(!1), r = j >= n + q, b.show()), this.opts.dropdownAutoWidth ? (x = a(".select2-results", b)[0], b.addClass("select2-drop-auto-width"), b.css("width", ""), q = b.outerWidth(!1) + (x.scrollHeight === x.clientHeight ? 0 : k.width), q > e ? e = q : q = e, r = j >= n + q) : this.container.removeClass("select2-drop-auto-width"), "static" !== this.body().css("position") && (t = this.body().offset(), m -= t.top, n -= t.left), r || (n = c.left + e - q), w = { left: n, width: e }, u ? (w.bottom = i - c.top, w.top = "auto", this.container.addClass("select2-drop-above"), b.addClass("select2-drop-above")) : (w.top = m, w.bottom = "auto", this.container.removeClass("select2-drop-above"), b.removeClass("select2-drop-above")), w = a.extend(w, K(this.opts.dropdownCss)), b.css(w) }, shouldOpen: function () { var b; return this.opened() ? !1 : this._enabled === !1 || this._readonly === !0 ? !1 : (b = a.Event("select2-opening"), this.opts.element.trigger(b), !b.isDefaultPrevented()) }, clearDropdownAlignmentPreference: function () { this.container.removeClass("select2-drop-above"), this.dropdown.removeClass("select2-drop-above") }, open: function () { return this.shouldOpen() ? (this.opening(), !0) : !1 }, opening: function () { var f, b = this.containerId, c = "scroll." + b, d = "resize." + b, e = "orientationchange." + b; this.container.addClass("select2-dropdown-open").addClass("select2-container-active"), this.clearDropdownAlignmentPreference(), this.dropdown[0] !== this.body().children().last()[0] && this.dropdown.detach().appendTo(this.body()), f = a("#select2-drop-mask"), 0 == f.length && (f = a(document.createElement("div")), f.attr("id", "select2-drop-mask").attr("class", "select2-drop-mask"), f.hide(), f.appendTo(this.body()), f.on("mousedown touchstart click", function (b) { var d, c = a("#select2-drop"); c.length > 0 && (d = c.data("select2"), d.opts.selectOnBlur && d.selectHighlighted({ noFocus: !0 }), d.close({ focus: !0 }), b.preventDefault(), b.stopPropagation()) })), this.dropdown.prev()[0] !== f[0] && this.dropdown.before(f), a("#select2-drop").removeAttr("id"), this.dropdown.attr("id", "select2-drop"), f.show(), this.positionDropdown(), this.dropdown.show(), this.positionDropdown(), this.dropdown.addClass("select2-drop-active"); var g = this; this.container.parents().add(window).each(function () { a(this).on(d + " " + c + " " + e, function () { g.positionDropdown() }) }) }, close: function () { if (this.opened()) { var b = this.containerId, c = "scroll." + b, d = "resize." + b, e = "orientationchange." + b; this.container.parents().add(window).each(function () { a(this).off(c).off(d).off(e) }), this.clearDropdownAlignmentPreference(), a("#select2-drop-mask").hide(), this.dropdown.removeAttr("id"), this.dropdown.hide(), this.container.removeClass("select2-dropdown-open").removeClass("select2-container-active"), this.results.empty(), this.clearSearch(), this.search.removeClass("select2-active"), this.opts.element.trigger(a.Event("select2-close")) } }, externalSearch: function (a) { this.open(), this.search.val(a), this.updateResults(!1) }, clearSearch: function () { }, getMaximumSelectionSize: function () { return K(this.opts.maximumSelectionSize) }, ensureHighlightVisible: function () { var c, d, e, f, g, h, i, b = this.results; if (d = this.highlight(), !(0 > d)) { if (0 == d) return b.scrollTop(0), void 0; c = this.findHighlightableChoices().find(".select2-result-label"), e = a(c[d]), f = e.offset().top + e.outerHeight(!0), d === c.length - 1 && (i = b.find("li.select2-more-results"), i.length > 0 && (f = i.offset().top + i.outerHeight(!0))), g = b.offset().top + b.outerHeight(!0), f > g && b.scrollTop(b.scrollTop() + (f - g)), h = e.offset().top - b.offset().top, 0 > h && "none" != e.css("display") && b.scrollTop(b.scrollTop() + h) } }, findHighlightableChoices: function () { return this.results.find(".select2-result-selectable:not(.select2-disabled, .select2-selected)") }, moveHighlight: function (b) { for (var c = this.findHighlightableChoices(), d = this.highlight() ; d > -1 && d < c.length;) { d += b; var e = a(c[d]); if (e.hasClass("select2-result-selectable") && !e.hasClass("select2-disabled") && !e.hasClass("select2-selected")) { this.highlight(d); break } } }, highlight: function (b) { var d, e, c = this.findHighlightableChoices(); return 0 === arguments.length ? o(c.filter(".select2-highlighted")[0], c.get()) : (b >= c.length && (b = c.length - 1), 0 > b && (b = 0), this.removeHighlight(), d = a(c[b]), d.addClass("select2-highlighted"), this.ensureHighlightVisible(), e = d.data("select2-data"), e && this.opts.element.trigger({ type: "select2-highlight", val: this.id(e), choice: e }), void 0) }, removeHighlight: function () { this.results.find(".select2-highlighted").removeClass("select2-highlighted") }, countSelectableResults: function () { return this.findHighlightableChoices().length }, highlightUnderEvent: function (b) { var c = a(b.target).closest(".select2-result-selectable"); if (c.length > 0 && !c.is(".select2-highlighted")) { var d = this.findHighlightableChoices(); this.highlight(d.index(c)) } else 0 == c.length && this.removeHighlight() }, loadMoreIfNeeded: function () { var c, a = this.results, b = a.find("li.select2-more-results"), d = this.resultsPage + 1, e = this, f = this.search.val(), g = this.context; 0 !== b.length && (c = b.offset().top - a.offset().top - a.height(), c <= this.opts.loadMorePadding && (b.addClass("select2-active"), this.opts.query({ element: this.opts.element, term: f, page: d, context: g, matcher: this.opts.matcher, callback: this.bind(function (c) { e.opened() && (e.opts.populateResults.call(this, a, c.results, { term: f, page: d, context: g }), e.postprocessResults(c, !1, !1), c.more === !0 ? (b.detach().appendTo(a).text(e.opts.formatLoadMore(d + 1)), window.setTimeout(function () { e.loadMoreIfNeeded() }, 10)) : b.remove(), e.positionDropdown(), e.resultsPage = d, e.context = c.context, this.opts.element.trigger({ type: "select2-loaded", items: c })) }) }))) }, tokenize: function () { }, updateResults: function (c) {
                function m() { d.removeClass("select2-active"), h.positionDropdown() } function n(a) { e.html(a), m() } var g, i, l, d = this.search, e = this.results, f = this.opts, h = this, j = d.val(), k = a.data(this.container, "select2-last-term"); if ((c === !0 || !k || !q(j, k)) && (a.data(this.container, "select2-last-term", j), c === !0 || this.showSearchInput !== !1 && this.opened())) {
                    l = ++this.queryCount; var o = this.getMaximumSelectionSize(); if (o >= 1 && (g = this.data(), a.isArray(g) && g.length >= o && J(f.formatSelectionTooBig, "formatSelectionTooBig"))) return n("<li class='select2-selection-limit'>" + f.formatSelectionTooBig(o) + "</li>"), void 0; if (d.val().length < f.minimumInputLength) return J(f.formatInputTooShort, "formatInputTooShort") ? n("<li class='select2-no-results'>" + f.formatInputTooShort(d.val(), f.minimumInputLength) + "</li>") : n(""), c && this.showSearch && this.showSearch(!0), void 0;
                    if (f.maximumInputLength && d.val().length > f.maximumInputLength) return J(f.formatInputTooLong, "formatInputTooLong") ? n("<li class='select2-no-results'>" + f.formatInputTooLong(d.val(), f.maximumInputLength) + "</li>") : n(""), void 0; f.formatSearching && 0 === this.findHighlightableChoices().length && n("<li class='select2-searching'>" + f.formatSearching() + "</li>"), d.addClass("select2-active"), this.removeHighlight(), i = this.tokenize(), i != b && null != i && d.val(i), this.resultsPage = 1, f.query({ element: f.element, term: d.val(), page: this.resultsPage, context: null, matcher: f.matcher, callback: this.bind(function (g) { var i; if (l == this.queryCount) { if (!this.opened()) return this.search.removeClass("select2-active"), void 0; if (this.context = g.context === b ? null : g.context, this.opts.createSearchChoice && "" !== d.val() && (i = this.opts.createSearchChoice.call(h, d.val(), g.results), i !== b && null !== i && h.id(i) !== b && null !== h.id(i) && 0 === a(g.results).filter(function () { return q(h.id(this), h.id(i)) }).length && g.results.unshift(i)), 0 === g.results.length && J(f.formatNoMatches, "formatNoMatches")) return n("<li class='select2-no-results'>" + f.formatNoMatches(d.val()) + "</li>"), void 0; e.empty(), h.opts.populateResults.call(this, e, g.results, { term: d.val(), page: this.resultsPage, context: null }), g.more === !0 && J(f.formatLoadMore, "formatLoadMore") && (e.append("<li class='select2-more-results'>" + h.opts.escapeMarkup(f.formatLoadMore(this.resultsPage)) + "</li>"), window.setTimeout(function () { h.loadMoreIfNeeded() }, 10)), this.postprocessResults(g, c), m(), this.opts.element.trigger({ type: "select2-loaded", items: g }) } }) })
                }
            }, cancel: function () { this.close() }, blur: function () { this.opts.selectOnBlur && this.selectHighlighted({ noFocus: !0 }), this.close(), this.container.removeClass("select2-container-active"), this.search[0] === document.activeElement && this.search.blur(), this.clearSearch(), this.selection.find(".select2-search-choice-focus").removeClass("select2-search-choice-focus") }, focusSearch: function () { y(this.search) }, selectHighlighted: function (a) { var b = this.highlight(), c = this.results.find(".select2-highlighted"), d = c.closest(".select2-result").data("select2-data"); d ? (this.highlight(b), this.onSelect(d, a)) : a && a.noFocus && this.close() }, getPlaceholder: function () { var a; return this.opts.element.attr("placeholder") || this.opts.element.attr("data-placeholder") || this.opts.element.data("placeholder") || this.opts.placeholder || ((a = this.getPlaceholderOption()) !== b ? a.text() : b) }, getPlaceholderOption: function () { if (this.select) { var a = this.select.children("option").first(); if (this.opts.placeholderOption !== b) return "first" === this.opts.placeholderOption && a || "function" == typeof this.opts.placeholderOption && this.opts.placeholderOption(this.select); if ("" === a.text() && "" === a.val()) return a } }, initContainerWidth: function () { function c() { var c, d, e, f, g, h; if ("off" === this.opts.width) return null; if ("element" === this.opts.width) return 0 === this.opts.element.outerWidth(!1) ? "auto" : this.opts.element.outerWidth(!1) + "px"; if ("copy" === this.opts.width || "resolve" === this.opts.width) { if (c = this.opts.element.attr("style"), c !== b) for (d = c.split(";"), f = 0, g = d.length; g > f; f += 1) if (h = d[f].replace(/\s/g, ""), e = h.match(/^width:(([-+]?([0-9]*\.)?[0-9]+)(px|em|ex|%|in|cm|mm|pt|pc))/i), null !== e && e.length >= 1) return e[1]; return "resolve" === this.opts.width ? (c = this.opts.element.css("width"), c.indexOf("%") > 0 ? c : 0 === this.opts.element.outerWidth(!1) ? "auto" : this.opts.element.outerWidth(!1) + "px") : null } return a.isFunction(this.opts.width) ? this.opts.width() : this.opts.width } var d = c.call(this); null !== d && this.container.css("width", d) }
        }), e = N(d, { createContainer: function () { var b = a(document.createElement("div")).attr({ "class": "select2-container" }).html(["<a href='javascript:void(0)' onclick='return false;' class='select2-choice' tabindex='-1'>", "   <span class='select2-chosen'>&nbsp;</span><abbr class='select2-search-choice-close'></abbr>", "   <span class='select2-arrow'><b></b></span>", "</a>", "<input class='select2-focusser select2-offscreen' type='text'/>", "<div class='select2-drop select2-display-none'>", "   <div class='select2-search'>", "       <input type='text' autocomplete='off' autocorrect='off' autocapitalize='off' spellcheck='false' class='select2-input'/>", "   </div>", "   <ul class='select2-results'>", "   </ul>", "</div>"].join("")); return b }, enableInterface: function () { this.parent.enableInterface.apply(this, arguments) && this.focusser.prop("disabled", !this.isInterfaceEnabled()) }, opening: function () { var c, d, e; this.opts.minimumResultsForSearch >= 0 && this.showSearch(!0), this.parent.opening.apply(this, arguments), this.showSearchInput !== !1 && this.search.val(this.focusser.val()), this.search.focus(), c = this.search.get(0), c.createTextRange ? (d = c.createTextRange(), d.collapse(!1), d.select()) : c.setSelectionRange && (e = this.search.val().length, c.setSelectionRange(e, e)), "" === this.search.val() && this.nextSearchTerm != b && (this.search.val(this.nextSearchTerm), this.search.select()), this.focusser.prop("disabled", !0).val(""), this.updateResults(!0), this.opts.element.trigger(a.Event("select2-open")) }, close: function (a) { this.opened() && (this.parent.close.apply(this, arguments), a = a || { focus: !0 }, this.focusser.removeAttr("disabled"), a.focus && this.focusser.focus()) }, focus: function () { this.opened() ? this.close() : (this.focusser.removeAttr("disabled"), this.focusser.focus()) }, isFocused: function () { return this.container.hasClass("select2-container-active") }, cancel: function () { this.parent.cancel.apply(this, arguments), this.focusser.removeAttr("disabled"), this.focusser.focus() }, destroy: function () { a("label[for='" + this.focusser.attr("id") + "']").attr("for", this.opts.element.attr("id")), this.parent.destroy.apply(this, arguments) }, initContainer: function () { var b, d = this.container, e = this.dropdown; this.opts.minimumResultsForSearch < 0 ? this.showSearch(!1) : this.showSearch(!0), this.selection = b = d.find(".select2-choice"), this.focusser = d.find(".select2-focusser"), this.focusser.attr("id", "s2id_autogen" + g()), a("label[for='" + this.opts.element.attr("id") + "']").attr("for", this.focusser.attr("id")), this.focusser.attr("tabindex", this.elementTabIndex), this.search.on("keydown", this.bind(function (a) { if (this.isInterfaceEnabled()) { if (a.which === c.PAGE_UP || a.which === c.PAGE_DOWN) return A(a), void 0; switch (a.which) { case c.UP: case c.DOWN: return this.moveHighlight(a.which === c.UP ? -1 : 1), A(a), void 0; case c.ENTER: return this.selectHighlighted(), A(a), void 0; case c.TAB: return this.selectHighlighted({ noFocus: !0 }), void 0; case c.ESC: return this.cancel(a), A(a), void 0 } } })), this.search.on("blur", this.bind(function () { document.activeElement === this.body().get(0) && window.setTimeout(this.bind(function () { this.search.focus() }), 0) })), this.focusser.on("keydown", this.bind(function (a) { if (this.isInterfaceEnabled() && a.which !== c.TAB && !c.isControl(a) && !c.isFunctionKey(a) && a.which !== c.ESC) { if (this.opts.openOnEnter === !1 && a.which === c.ENTER) return A(a), void 0; if (a.which == c.DOWN || a.which == c.UP || a.which == c.ENTER && this.opts.openOnEnter) { if (a.altKey || a.ctrlKey || a.shiftKey || a.metaKey) return; return this.open(), A(a), void 0 } return a.which == c.DELETE || a.which == c.BACKSPACE ? (this.opts.allowClear && this.clear(), A(a), void 0) : void 0 } })), t(this.focusser), this.focusser.on("keyup-change input", this.bind(function (a) { if (this.opts.minimumResultsForSearch >= 0) { if (a.stopPropagation(), this.opened()) return; this.open() } })), b.on("mousedown", "abbr", this.bind(function (a) { this.isInterfaceEnabled() && (this.clear(), B(a), this.close(), this.selection.focus()) })), b.on("mousedown", this.bind(function (b) { this.container.hasClass("select2-container-active") || this.opts.element.trigger(a.Event("select2-focus")), this.opened() ? this.close() : this.isInterfaceEnabled() && this.open(), A(b) })), e.on("mousedown", this.bind(function () { this.search.focus() })), b.on("focus", this.bind(function (a) { A(a) })), this.focusser.on("focus", this.bind(function () { this.container.hasClass("select2-container-active") || this.opts.element.trigger(a.Event("select2-focus")), this.container.addClass("select2-container-active") })).on("blur", this.bind(function () { this.opened() || (this.container.removeClass("select2-container-active"), this.opts.element.trigger(a.Event("select2-blur"))) })), this.search.on("focus", this.bind(function () { this.container.hasClass("select2-container-active") || this.opts.element.trigger(a.Event("select2-focus")), this.container.addClass("select2-container-active") })), this.initContainerWidth(), this.opts.element.addClass("select2-offscreen"), this.setPlaceholder() }, clear: function (b) { var c = this.selection.data("select2-data"); if (c) { var d = a.Event("select2-clearing"); if (this.opts.element.trigger(d), d.isDefaultPrevented()) return; var e = this.getPlaceholderOption(); this.opts.element.val(e ? e.val() : ""), this.selection.find(".select2-chosen").empty(), this.selection.removeData("select2-data"), this.setPlaceholder(), b !== !1 && (this.opts.element.trigger({ type: "select2-removed", val: this.id(c), choice: c }), this.triggerChange({ removed: c })) } }, initSelection: function () { if (this.isPlaceholderOptionSelected()) this.updateSelection(null), this.close(), this.setPlaceholder(); else { var c = this; this.opts.initSelection.call(null, this.opts.element, function (a) { a !== b && null !== a && (c.updateSelection(a), c.close(), c.setPlaceholder()) }) } }, isPlaceholderOptionSelected: function () { var a; return this.getPlaceholder() ? (a = this.getPlaceholderOption()) !== b && a.prop("selected") || "" === this.opts.element.val() || this.opts.element.val() === b || null === this.opts.element.val() : !1 }, prepareOpts: function () { var b = this.parent.prepareOpts.apply(this, arguments), c = this; return "select" === b.element.get(0).tagName.toLowerCase() ? b.initSelection = function (a, b) { var d = a.find("option").filter(function () { return this.selected }); b(c.optionToData(d)) } : "data" in b && (b.initSelection = b.initSelection || function (c, d) { var e = c.val(), f = null; b.query({ matcher: function (a, c, d) { var g = q(e, b.id(d)); return g && (f = d), g }, callback: a.isFunction(d) ? function () { d(f) } : a.noop }) }), b }, getPlaceholder: function () { return this.select && this.getPlaceholderOption() === b ? b : this.parent.getPlaceholder.apply(this, arguments) }, setPlaceholder: function () { var a = this.getPlaceholder(); if (this.isPlaceholderOptionSelected() && a !== b) { if (this.select && this.getPlaceholderOption() === b) return; this.selection.find(".select2-chosen").html(this.opts.escapeMarkup(a)), this.selection.addClass("select2-default"), this.container.removeClass("select2-allowclear") } }, postprocessResults: function (a, b, c) { var d = 0, e = this; if (this.findHighlightableChoices().each2(function (a, b) { return q(e.id(b.data("select2-data")), e.opts.element.val()) ? (d = a, !1) : void 0 }), c !== !1 && (b === !0 && d >= 0 ? this.highlight(d) : this.highlight(0)), b === !0) { var g = this.opts.minimumResultsForSearch; g >= 0 && this.showSearch(L(a.results) >= g) } }, showSearch: function (b) { this.showSearchInput !== b && (this.showSearchInput = b, this.dropdown.find(".select2-search").toggleClass("select2-search-hidden", !b), this.dropdown.find(".select2-search").toggleClass("select2-offscreen", !b), a(this.dropdown, this.container).toggleClass("select2-with-searchbox", b)) }, onSelect: function (a, b) { if (this.triggerSelect(a)) { var c = this.opts.element.val(), d = this.data(); this.opts.element.val(this.id(a)), this.updateSelection(a), this.opts.element.trigger({ type: "select2-selected", val: this.id(a), choice: a }), this.nextSearchTerm = this.opts.nextSearchTerm(a, this.search.val()), this.close(), b && b.noFocus || this.focusser.focus(), q(c, this.id(a)) || this.triggerChange({ added: a, removed: d }) } }, updateSelection: function (a) { var d, e, c = this.selection.find(".select2-chosen"); this.selection.data("select2-data", a), c.empty(), null !== a && (d = this.opts.formatSelection(a, c, this.opts.escapeMarkup)), d !== b && c.append(d), e = this.opts.formatSelectionCssClass(a, c), e !== b && c.addClass(e), this.selection.removeClass("select2-default"), this.opts.allowClear && this.getPlaceholder() !== b && this.container.addClass("select2-allowclear") }, val: function () { var a, c = !1, d = null, e = this, f = this.data(); if (0 === arguments.length) return this.opts.element.val(); if (a = arguments[0], arguments.length > 1 && (c = arguments[1]), this.select) this.select.val(a).find("option").filter(function () { return this.selected }).each2(function (a, b) { return d = e.optionToData(b), !1 }), this.updateSelection(d), this.setPlaceholder(), c && this.triggerChange({ added: d, removed: f }); else { if (!a && 0 !== a) return this.clear(c), void 0; if (this.opts.initSelection === b) throw new Error("cannot call val() if initSelection() is not defined"); this.opts.element.val(a), this.opts.initSelection(this.opts.element, function (a) { e.opts.element.val(a ? e.id(a) : ""), e.updateSelection(a), e.setPlaceholder(), c && e.triggerChange({ added: a, removed: f }) }) } }, clearSearch: function () { this.search.val(""), this.focusser.val("") }, data: function (a) { var c, d = !1; return 0 === arguments.length ? (c = this.selection.data("select2-data"), c == b && (c = null), c) : (arguments.length > 1 && (d = arguments[1]), a ? (c = this.data(), this.opts.element.val(a ? this.id(a) : ""), this.updateSelection(a), d && this.triggerChange({ added: a, removed: c })) : this.clear(d), void 0) } }), f = N(d, { createContainer: function () { var b = a(document.createElement("div")).attr({ "class": "select2-container select2-container-multi" }).html(["<ul class='select2-choices'>", "  <li class='select2-search-field'>", "    <input type='text' autocomplete='off' autocorrect='off' autocapitalize='off' spellcheck='false' class='select2-input'>", "  </li>", "</ul>", "<div class='select2-drop select2-drop-multi select2-display-none'>", "   <ul class='select2-results'>", "   </ul>", "</div>"].join("")); return b }, prepareOpts: function () { var b = this.parent.prepareOpts.apply(this, arguments), c = this; return "select" === b.element.get(0).tagName.toLowerCase() ? b.initSelection = function (a, b) { var d = []; a.find("option").filter(function () { return this.selected }).each2(function (a, b) { d.push(c.optionToData(b)) }), b(d) } : "data" in b && (b.initSelection = b.initSelection || function (c, d) { var e = r(c.val(), b.separator), f = []; b.query({ matcher: function (c, d, g) { var h = a.grep(e, function (a) { return q(a, b.id(g)) }).length; return h && f.push(g), h }, callback: a.isFunction(d) ? function () { for (var a = [], c = 0; c < e.length; c++) for (var g = e[c], h = 0; h < f.length; h++) { var i = f[h]; if (q(g, b.id(i))) { a.push(i), f.splice(h, 1); break } } d(a) } : a.noop }) }), b }, selectChoice: function (a) { var b = this.container.find(".select2-search-choice-focus"); b.length && a && a[0] == b[0] || (b.length && this.opts.element.trigger("choice-deselected", b), b.removeClass("select2-search-choice-focus"), a && a.length && (this.close(), a.addClass("select2-search-choice-focus"), this.opts.element.trigger("choice-selected", a))) }, destroy: function () { a("label[for='" + this.search.attr("id") + "']").attr("for", this.opts.element.attr("id")), this.parent.destroy.apply(this, arguments) }, initContainer: function () { var d, b = ".select2-choices"; this.searchContainer = this.container.find(".select2-search-field"), this.selection = d = this.container.find(b); var e = this; this.selection.on("click", ".select2-search-choice:not(.select2-locked)", function () { e.search[0].focus(), e.selectChoice(a(this)) }), this.search.attr("id", "s2id_autogen" + g()), a("label[for='" + this.opts.element.attr("id") + "']").attr("for", this.search.attr("id")), this.search.on("input paste", this.bind(function () { this.isInterfaceEnabled() && (this.opened() || this.open()) })), this.search.attr("tabindex", this.elementTabIndex), this.keydowns = 0, this.search.on("keydown", this.bind(function (a) { if (this.isInterfaceEnabled()) { ++this.keydowns; var b = d.find(".select2-search-choice-focus"), e = b.prev(".select2-search-choice:not(.select2-locked)"), f = b.next(".select2-search-choice:not(.select2-locked)"), g = z(this.search); if (b.length && (a.which == c.LEFT || a.which == c.RIGHT || a.which == c.BACKSPACE || a.which == c.DELETE || a.which == c.ENTER)) { var h = b; return a.which == c.LEFT && e.length ? h = e : a.which == c.RIGHT ? h = f.length ? f : null : a.which === c.BACKSPACE ? (this.unselect(b.first()), this.search.width(10), h = e.length ? e : f) : a.which == c.DELETE ? (this.unselect(b.first()), this.search.width(10), h = f.length ? f : null) : a.which == c.ENTER && (h = null), this.selectChoice(h), A(a), h && h.length || this.open(), void 0 } if ((a.which === c.BACKSPACE && 1 == this.keydowns || a.which == c.LEFT) && 0 == g.offset && !g.length) return this.selectChoice(d.find(".select2-search-choice:not(.select2-locked)").last()), A(a), void 0; if (this.selectChoice(null), this.opened()) switch (a.which) { case c.UP: case c.DOWN: return this.moveHighlight(a.which === c.UP ? -1 : 1), A(a), void 0; case c.ENTER: return this.selectHighlighted(), A(a), void 0; case c.TAB: return this.selectHighlighted({ noFocus: !0 }), this.close(), void 0; case c.ESC: return this.cancel(a), A(a), void 0 } if (a.which !== c.TAB && !c.isControl(a) && !c.isFunctionKey(a) && a.which !== c.BACKSPACE && a.which !== c.ESC) { if (a.which === c.ENTER) { if (this.opts.openOnEnter === !1) return; if (a.altKey || a.ctrlKey || a.shiftKey || a.metaKey) return } this.open(), (a.which === c.PAGE_UP || a.which === c.PAGE_DOWN) && A(a), a.which === c.ENTER && A(a) } } })), this.search.on("keyup", this.bind(function () { this.keydowns = 0, this.resizeSearch() })), this.search.on("blur", this.bind(function (b) { this.container.removeClass("select2-container-active"), this.search.removeClass("select2-focused"), this.selectChoice(null), this.opened() || this.clearSearch(), b.stopImmediatePropagation(), this.opts.element.trigger(a.Event("select2-blur")) })), this.container.on("click", b, this.bind(function (b) { this.isInterfaceEnabled() && (a(b.target).closest(".select2-search-choice").length > 0 || (this.selectChoice(null), this.clearPlaceholder(), this.container.hasClass("select2-container-active") || this.opts.element.trigger(a.Event("select2-focus")), this.open(), this.focusSearch(), b.preventDefault())) })), this.container.on("focus", b, this.bind(function () { this.isInterfaceEnabled() && (this.container.hasClass("select2-container-active") || this.opts.element.trigger(a.Event("select2-focus")), this.container.addClass("select2-container-active"), this.dropdown.addClass("select2-drop-active"), this.clearPlaceholder()) })), this.initContainerWidth(), this.opts.element.addClass("select2-offscreen"), this.clearSearch() }, enableInterface: function () { this.parent.enableInterface.apply(this, arguments) && this.search.prop("disabled", !this.isInterfaceEnabled()) }, initSelection: function () { if ("" === this.opts.element.val() && "" === this.opts.element.text() && (this.updateSelection([]), this.close(), this.clearSearch()), this.select || "" !== this.opts.element.val()) { var c = this; this.opts.initSelection.call(null, this.opts.element, function (a) { a !== b && null !== a && (c.updateSelection(a), c.close(), c.clearSearch()) }) } }, clearSearch: function () { var a = this.getPlaceholder(), c = this.getMaxSearchWidth(); a !== b && 0 === this.getVal().length && this.search.hasClass("select2-focused") === !1 ? (this.search.val(a).addClass("select2-default"), this.search.width(c > 0 ? c : this.container.css("width"))) : this.search.val("").width(10) }, clearPlaceholder: function () { this.search.hasClass("select2-default") && this.search.val("").removeClass("select2-default") }, opening: function () { this.clearPlaceholder(), this.resizeSearch(), this.parent.opening.apply(this, arguments), this.focusSearch(), this.updateResults(!0), this.search.focus(), this.opts.element.trigger(a.Event("select2-open")) }, close: function () { this.opened() && this.parent.close.apply(this, arguments) }, focus: function () { this.close(), this.search.focus() }, isFocused: function () { return this.search.hasClass("select2-focused") }, updateSelection: function (b) { var c = [], d = [], e = this; a(b).each(function () { o(e.id(this), c) < 0 && (c.push(e.id(this)), d.push(this)) }), b = d, this.selection.find(".select2-search-choice").remove(), a(b).each(function () { e.addSelectedChoice(this) }), e.postprocessResults() }, tokenize: function () { var a = this.search.val(); a = this.opts.tokenizer.call(this, a, this.data(), this.bind(this.onSelect), this.opts), null != a && a != b && (this.search.val(a), a.length > 0 && this.open()) }, onSelect: function (a, b) { this.triggerSelect(a) && (this.addSelectedChoice(a), this.opts.element.trigger({ type: "selected", val: this.id(a), choice: a }), (this.select || !this.opts.closeOnSelect) && this.postprocessResults(a, !1, this.opts.closeOnSelect === !0), this.opts.closeOnSelect ? (this.close(), this.search.width(10)) : this.countSelectableResults() > 0 ? (this.search.width(10), this.resizeSearch(), this.getMaximumSelectionSize() > 0 && this.val().length >= this.getMaximumSelectionSize() && this.updateResults(!0), this.positionDropdown()) : (this.close(), this.search.width(10)), this.triggerChange({ added: a }), b && b.noFocus || this.focusSearch()) }, cancel: function () { this.close(), this.focusSearch() }, addSelectedChoice: function (c) { var j, k, d = !c.locked, e = a("<li class='select2-search-choice'>    <div></div>    <a href='#' onclick='return false;' class='select2-search-choice-close' tabindex='-1'></a></li>"), f = a("<li class='select2-search-choice select2-locked'><div></div></li>"), g = d ? e : f, h = this.id(c), i = this.getVal(); j = this.opts.formatSelection(c, g.find("div"), this.opts.escapeMarkup), j != b && g.find("div").replaceWith("<div>" + j + "</div>"), k = this.opts.formatSelectionCssClass(c, g.find("div")), k != b && g.addClass(k), d && g.find(".select2-search-choice-close").on("mousedown", A).on("click dblclick", this.bind(function (b) { this.isInterfaceEnabled() && (a(b.target).closest(".select2-search-choice").fadeOut("fast", this.bind(function () { this.unselect(a(b.target)), this.selection.find(".select2-search-choice-focus").removeClass("select2-search-choice-focus"), this.close(), this.focusSearch() })).dequeue(), A(b)) })).on("focus", this.bind(function () { this.isInterfaceEnabled() && (this.container.addClass("select2-container-active"), this.dropdown.addClass("select2-drop-active")) })), g.data("select2-data", c), g.insertBefore(this.searchContainer), i.push(h), this.setVal(i) }, unselect: function (b) { var d, e, c = this.getVal(); if (b = b.closest(".select2-search-choice"), 0 === b.length) throw "Invalid argument: " + b + ". Must be .select2-search-choice"; if (d = b.data("select2-data")) { for (; (e = o(this.id(d), c)) >= 0;) c.splice(e, 1), this.setVal(c), this.select && this.postprocessResults(); var f = a.Event("select2-removing"); f.val = this.id(d), f.choice = d, this.opts.element.trigger(f), f.isDefaultPrevented() || (b.remove(), this.opts.element.trigger({ type: "select2-removed", val: this.id(d), choice: d }), this.triggerChange({ removed: d })) } }, postprocessResults: function (a, b, c) { var d = this.getVal(), e = this.results.find(".select2-result"), f = this.results.find(".select2-result-with-children"), g = this; e.each2(function (a, b) { var c = g.id(b.data("select2-data")); o(c, d) >= 0 && (b.addClass("select2-selected"), b.find(".select2-result-selectable").addClass("select2-selected")) }), f.each2(function (a, b) { b.is(".select2-result-selectable") || 0 !== b.find(".select2-result-selectable:not(.select2-selected)").length || b.addClass("select2-selected") }), -1 == this.highlight() && c !== !1 && g.highlight(0), !this.opts.createSearchChoice && !e.filter(".select2-result:not(.select2-selected)").length > 0 && (!a || a && !a.more && 0 === this.results.find(".select2-no-results").length) && J(g.opts.formatNoMatches, "formatNoMatches") && this.results.append("<li class='select2-no-results'>" + g.opts.formatNoMatches(g.search.val()) + "</li>") }, getMaxSearchWidth: function () { return this.selection.width() - s(this.search) }, resizeSearch: function () { var a, b, c, d, e, f = s(this.search); a = C(this.search) + 10, b = this.search.offset().left, c = this.selection.width(), d = this.selection.offset().left, e = c - (b - d) - f, a > e && (e = c - f), 40 > e && (e = c - f), 0 >= e && (e = a), this.search.width(Math.floor(e)) }, getVal: function () { var a; return this.select ? (a = this.select.val(), null === a ? [] : a) : (a = this.opts.element.val(), r(a, this.opts.separator)) }, setVal: function (b) { var c; this.select ? this.select.val(b) : (c = [], a(b).each(function () { o(this, c) < 0 && c.push(this) }), this.opts.element.val(0 === c.length ? "" : c.join(this.opts.separator))) }, buildChangeDetails: function (a, b) { for (var b = b.slice(0), a = a.slice(0), c = 0; c < b.length; c++) for (var d = 0; d < a.length; d++) q(this.opts.id(b[c]), this.opts.id(a[d])) && (b.splice(c, 1), c > 0 && c--, a.splice(d, 1), d--); return { added: b, removed: a } }, val: function (c, d) { var e, f = this; if (0 === arguments.length) return this.getVal(); if (e = this.data(), e.length || (e = []), !c && 0 !== c) return this.opts.element.val(""), this.updateSelection([]), this.clearSearch(), d && this.triggerChange({ added: this.data(), removed: e }), void 0; if (this.setVal(c), this.select) this.opts.initSelection(this.select, this.bind(this.updateSelection)), d && this.triggerChange(this.buildChangeDetails(e, this.data())); else { if (this.opts.initSelection === b) throw new Error("val() cannot be called if initSelection() is not defined"); this.opts.initSelection(this.opts.element, function (b) { var c = a.map(b, f.id); f.setVal(c), f.updateSelection(b), f.clearSearch(), d && f.triggerChange(f.buildChangeDetails(e, f.data())) }) } this.clearSearch() }, onSortStart: function () { if (this.select) throw new Error("Sorting of elements is not supported when attached to <select>. Attach to <input type='hidden'/> instead."); this.search.width(0), this.searchContainer.hide() }, onSortEnd: function () { var b = [], c = this; this.searchContainer.show(), this.searchContainer.appendTo(this.searchContainer.parent()), this.resizeSearch(), this.selection.find(".select2-search-choice").each(function () { b.push(c.opts.id(a(this).data("select2-data"))) }), this.setVal(b), this.triggerChange() }, data: function (b, c) { var e, f, d = this; return 0 === arguments.length ? this.selection.find(".select2-search-choice").map(function () { return a(this).data("select2-data") }).get() : (f = this.data(), b || (b = []), e = a.map(b, function (a) { return d.opts.id(a) }), this.setVal(e), this.updateSelection(b), this.clearSearch(), c && this.triggerChange(this.buildChangeDetails(f, this.data())), void 0) } }), a.fn.select2 = function () { var d, g, h, i, j, c = Array.prototype.slice.call(arguments, 0), k = ["val", "destroy", "opened", "open", "close", "focus", "isFocused", "container", "dropdown", "onSortStart", "onSortEnd", "enable", "disable", "readonly", "positionDropdown", "data", "search"], l = ["opened", "isFocused", "container", "dropdown"], m = ["val", "data"], n = { search: "externalSearch" }; return this.each(function () { if (0 === c.length || "object" == typeof c[0]) d = 0 === c.length ? {} : a.extend({}, c[0]), d.element = a(this), "select" === d.element.get(0).tagName.toLowerCase() ? j = d.element.prop("multiple") : (j = d.multiple || !1, "tags" in d && (d.multiple = j = !0)), g = j ? new f : new e, g.init(d); else { if ("string" != typeof c[0]) throw "Invalid arguments to select2 plugin: " + c; if (o(c[0], k) < 0) throw "Unknown method: " + c[0]; if (i = b, g = a(this).data("select2"), g === b) return; if (h = c[0], "container" === h ? i = g.container : "dropdown" === h ? i = g.dropdown : (n[h] && (h = n[h]), i = g[h].apply(g, c.slice(1))), o(c[0], l) >= 0 || o(c[0], m) && 1 == c.length) return !1 } }), i === b ? this : i }, a.fn.select2.defaults = { width: "copy", loadMorePadding: 0, closeOnSelect: !0, openOnEnter: !0, containerCss: {}, dropdownCss: {}, containerCssClass: "", dropdownCssClass: "", formatResult: function (a, b, c, d) { var e = []; return E(a.text, c.term, e, d), e.join("") }, formatSelection: function (a, c, d) { return a ? d(a.text) : b }, sortResults: function (a) { return a }, formatResultCssClass: function () { return b }, formatSelectionCssClass: function () { return b }, formatNoMatches: function () { return "No matches found" }, formatInputTooShort: function (a, b) { var c = b - a.length; return "Please enter " + c + " more character" + (1 == c ? "" : "s") }, formatInputTooLong: function (a, b) { var c = a.length - b; return "Please delete " + c + " character" + (1 == c ? "" : "s") }, formatSelectionTooBig: function (a) { return "You can only select " + a + " item" + (1 == a ? "" : "s") }, formatLoadMore: function () { return "Loading more results..." }, formatSearching: function () { return "Searching..." }, minimumResultsForSearch: 0, minimumInputLength: 0, maximumInputLength: null, maximumSelectionSize: 0, id: function (a) { return a.id }, matcher: function (a, b) { return n("" + b).toUpperCase().indexOf(n("" + a).toUpperCase()) >= 0 }, separator: ",", tokenSeparators: [], tokenizer: M, escapeMarkup: F, blurOnChange: !1, selectOnBlur: !1, adaptContainerCssClass: function (a) { return a }, adaptDropdownCssClass: function () { return null }, nextSearchTerm: function () { return b } }, a.fn.select2.ajaxDefaults = { transport: a.ajax, params: { type: "GET", cache: !1, dataType: "json" } }, window.Select2 = { query: { ajax: G, local: H, tags: I }, util: { debounce: v, markMatch: E, escapeMarkup: F, stripDiacritics: n }, "class": { "abstract": d, single: e, multi: f } }
    }
}(jQuery);

/*global jQuery */
/*!
* FitText.js 1.2
*
* Copyright 2011, Dave Rupert http://daverupert.com
* Released under the WTFPL license
* http://sam.zoy.org/wtfpl/
*
* Date: Thu May 05 14:23:00 2011 -0600
*/

(function( $ ){

  $.fn.fitText = function( kompressor, options ) {

    // Setup options
    var compressor = kompressor || 1,
        settings = $.extend({
          'minFontSize' : Number.NEGATIVE_INFINITY,
          'maxFontSize' : Number.POSITIVE_INFINITY
        }, options);

    return this.each(function(){

      // Store the object
      var $this = $(this);

      // Resizer() resizes items based on the object width divided by the compressor * 10
      var resizer = function () {
        $this.css('font-size', Math.max(Math.min($this.width() / (compressor*10), parseFloat(settings.maxFontSize)), parseFloat(settings.minFontSize)));
      };

      // Call once to set.
      resizer();

      // Call on resize. Opera debounces their resize by default.
      $(window).on('resize.fittext orientationchange.fittext', resizer);

    });

  };

})(jQuery);

/*
	Masked Input plugin for jQuery
	Copyright (c) 2007-2013 Josh Bush (digitalbush.com)
	Licensed under the MIT license (http://digitalbush.com/projects/masked-input-plugin/#license)
	Version: 1.3.1
*/
(function (e) { function t() { var e = document.createElement("input"), t = "onpaste"; return e.setAttribute(t, ""), "function" == typeof e[t] ? "paste" : "input" } var n, a = t() + ".mask", r = navigator.userAgent, i = /iphone/i.test(r), o = /android/i.test(r); e.mask = { definitions: { 9: "[0-9]", a: "[A-Za-z]", "*": "[A-Za-z0-9]" }, dataName: "rawMaskFn", placeholder: "_" }, e.fn.extend({ caret: function (e, t) { var n; if (0 !== this.length && !this.is(":hidden")) return "number" == typeof e ? (t = "number" == typeof t ? t : e, this.each(function () { this.setSelectionRange ? this.setSelectionRange(e, t) : this.createTextRange && (n = this.createTextRange(), n.collapse(!0), n.moveEnd("character", t), n.moveStart("character", e), n.select()) })) : (this[0].setSelectionRange ? (e = this[0].selectionStart, t = this[0].selectionEnd) : document.selection && document.selection.createRange && (n = document.selection.createRange(), e = 0 - n.duplicate().moveStart("character", -1e5), t = e + n.text.length), { begin: e, end: t }) }, unmask: function () { return this.trigger("unmask") }, mask: function (t, r) { var c, l, s, u, f, h; return !t && this.length > 0 ? (c = e(this[0]), c.data(e.mask.dataName)()) : (r = e.extend({ placeholder: e.mask.placeholder, completed: null }, r), l = e.mask.definitions, s = [], u = h = t.length, f = null, e.each(t.split(""), function (e, t) { "?" == t ? (h--, u = e) : l[t] ? (s.push(RegExp(l[t])), null === f && (f = s.length - 1)) : s.push(null) }), this.trigger("unmask").each(function () { function c(e) { for (; h > ++e && !s[e];); return e } function d(e) { for (; --e >= 0 && !s[e];); return e } function m(e, t) { var n, a; if (!(0 > e)) { for (n = e, a = c(t) ; h > n; n++) if (s[n]) { if (!(h > a && s[n].test(R[a]))) break; R[n] = R[a], R[a] = r.placeholder, a = c(a) } b(), x.caret(Math.max(f, e)) } } function p(e) { var t, n, a, i; for (t = e, n = r.placeholder; h > t; t++) if (s[t]) { if (a = c(t), i = R[t], R[t] = n, !(h > a && s[a].test(i))) break; n = i } } function g(e) { var t, n, a, r = e.which; 8 === r || 46 === r || i && 127 === r ? (t = x.caret(), n = t.begin, a = t.end, 0 === a - n && (n = 46 !== r ? d(n) : a = c(n - 1), a = 46 === r ? c(a) : a), k(n, a), m(n, a - 1), e.preventDefault()) : 27 == r && (x.val(S), x.caret(0, y()), e.preventDefault()) } function v(t) { var n, a, i, l = t.which, u = x.caret(); t.ctrlKey || t.altKey || t.metaKey || 32 > l || l && (0 !== u.end - u.begin && (k(u.begin, u.end), m(u.begin, u.end - 1)), n = c(u.begin - 1), h > n && (a = String.fromCharCode(l), s[n].test(a) && (p(n), R[n] = a, b(), i = c(n), o ? setTimeout(e.proxy(e.fn.caret, x, i), 0) : x.caret(i), r.completed && i >= h && r.completed.call(x))), t.preventDefault()) } function k(e, t) { var n; for (n = e; t > n && h > n; n++) s[n] && (R[n] = r.placeholder) } function b() { x.val(R.join("")) } function y(e) { var t, n, a = x.val(), i = -1; for (t = 0, pos = 0; h > t; t++) if (s[t]) { for (R[t] = r.placeholder; pos++ < a.length;) if (n = a.charAt(pos - 1), s[t].test(n)) { R[t] = n, i = t; break } if (pos > a.length) break } else R[t] === a.charAt(pos) && t !== u && (pos++, i = t); return e ? b() : u > i + 1 ? (x.val(""), k(0, h)) : (b(), x.val(x.val().substring(0, i + 1))), u ? t : f } var x = e(this), R = e.map(t.split(""), function (e) { return "?" != e ? l[e] ? r.placeholder : e : void 0 }), S = x.val(); x.data(e.mask.dataName, function () { return e.map(R, function (e, t) { return s[t] && e != r.placeholder ? e : null }).join("") }), x.attr("readonly") || x.one("unmask", function () { x.unbind(".mask").removeData(e.mask.dataName) }).bind("focus.mask", function () { clearTimeout(n); var e; S = x.val(), e = y(), n = setTimeout(function () { b(), e == t.length ? x.caret(0, e) : x.caret(e) }, 10) }).bind("blur.mask", function () { y(), x.val() != S && x.change() }).bind("keydown.mask", g).bind("keypress.mask", v).bind(a, function () { setTimeout(function () { var e = y(!0); x.caret(e), r.completed && e == x.val().length && r.completed.call(x) }, 0) }), y() })) } }) })(jQuery);





/*
copyright (c) 2008 Wouter van der Graaf, all rights reserved

css3-mediaqueries.js - CSS Helper and CSS3 Media Queries Enabler

author: Wouter van der Graaf <woutervandergraaf at gmail com>
version: 0.9 (20091001)
license: MIT
website: http://woutervandergraaf.nl/css3-mediaqueries-js/

W3C spec: http://www.w3.org/TR/css3-mediaqueries/

Note: use of embedded <style> is not recommended when using media queries, because IE  has no way of returning the raw literal css text from a <style> element.
*/


// true prototypal inheritance (http://javascript.crockford.com/prototypal.html)
if (typeof Object.create !== 'function') {
    Object.create = function (o) {
        function F() {}
        F.prototype = o;
        return new F();
    };
}


// user agent sniffing shortcuts
var ua = {
    toString: function () {
        return navigator.userAgent;
    },
    test: function (s) {
        return this.toString().toLowerCase().indexOf(s.toLowerCase()) > -1;
    }
};
ua.version = (ua.toString().toLowerCase().match(/[\s\S]+(?:rv|it|ra|ie)[\/: ]([\d.]+)/) || [])[1];
ua.webkit = ua.test('webkit');
ua.gecko = ua.test('gecko') && !ua.webkit;
ua.opera = ua.test('opera');
ua.ie = ua.test('msie') && !ua.opera;
ua.ie6 = ua.ie && document.compatMode && typeof document.documentElement.style.maxHeight === 'undefined';
ua.ie7 = ua.ie && document.documentElement && typeof document.documentElement.style.maxHeight !== 'undefined' && typeof XDomainRequest === 'undefined';
ua.ie8 = ua.ie && typeof XDomainRequest !== 'undefined';



// initialize when DOM content is loaded
var domReady = function () {
    var fns = [];
    var init = function () {
        if (!arguments.callee.done) { // run init functions once
            arguments.callee.done = true;
            for (var i = 0; i < fns.length; i++) {
                fns[i]();
            }
        }
    };

    // listeners for different browsers
    if (document.addEventListener) {
        document.addEventListener('DOMContentLoaded', init, false);
    }
    if (ua.ie) {
        (function () {
            try {
                // throws errors until after ondocumentready
                document.documentElement.doScroll('left');

                // If we are in an iframe, the above does not work properly.
        // Trying to access the length attribute of document.body, however,
        // does throw an error until ondocumentready, fixing this issue.
        document.body.length;
            }
            catch (e) {
                setTimeout(arguments.callee, 50);
                return;
            }
            // no errors, fire
            init();
        })();
        // trying to always fire before onload
        document.onreadystatechange = function () {
            if (document.readyState === 'complete') {
                document.onreadystatechange = null;
                init();
            }
        };
    }
    if (ua.webkit && document.readyState) {
        (function () {
            if (document.readyState !== 'loading') {
                init();
            }
            else {
                setTimeout(arguments.callee, 10);
            }
        })();
    }
    window.onload = init; // fallback

 return function (fn) { // add fn to init functions
    if (typeof fn === 'function') {
      // If DOM ready has already been fired, fire the function
      // right away.
      if(init.done) {
        fn();
      } else {
        // Add to the queue
        fns[fns.length] = fn;
      }
    }
    return fn;
  };
}();

// helper library for parsing css to objects
var cssHelper = function () {

    var regExp = {
        BLOCKS: /[^\s{][^{]*\{(?:[^{}]*\{[^{}]*\}[^{}]*|[^{}]*)*\}/g,
        BLOCKS_INSIDE: /[^\s{][^{]*\{[^{}]*\}/g,
        DECLARATIONS: /[a-zA-Z\-]+[^;]*:[^;]+;/g,
        RELATIVE_URLS: /url\(['"]?([^\/\)'"][^:\)'"]+)['"]?\)/g,
        // strip whitespace and comments, @import is evil
        REDUNDANT_COMPONENTS: /(?:\/\*([^*\\\\]|\*(?!\/))+\*\/|@import[^;]+;|@-moz-document\s*url-prefix\(\)\s*{(([^{}])+{([^{}])+}([^{}])+)+})/g,
        REDUNDANT_WHITESPACE: /\s*(,|:|;|\{|\})\s*/g,
        MORE_WHITESPACE: /\s{2,}/g,
        FINAL_SEMICOLONS: /;\}/g,
        NOT_WHITESPACE: /\S+/g
    };

    var parsed, parsing = false;

    var waiting = [];
    var wait = function (fn) {
        if (typeof fn === 'function') {
            waiting[waiting.length] = fn;
        }
    };
    var ready = function () {
        for (var i = 0; i < waiting.length; i++) {
            waiting[i](parsed);
        }
    };
    var events = {};
    var broadcast = function (n, v) {
        if (events[n]) {
            var listeners = events[n].listeners;
            if (listeners) {
                for (var i = 0; i < listeners.length; i++) {
                    listeners[i](v);
                }
            }
        }
    };

    var requestText = function (url, fnSuccess, fnFailure) {
        if (ua.ie && !window.XMLHttpRequest) {
            window.XMLHttpRequest = function () {
                return new ActiveXObject('Microsoft.XMLHTTP');
            };
        }
        if (!XMLHttpRequest) {
            return '';
        }
        var r = new XMLHttpRequest();
        try {
            r.open('get', url, true);
            r.setRequestHeader('X_REQUESTED_WITH', 'XMLHttpRequest');
        }
        catch (e) {
            fnFailure();
            return;
        }
        var done = false;
        setTimeout(function () {
            done = true;
        }, 5000);
        document.documentElement.style.cursor = 'progress';
        r.onreadystatechange = function () {
            if (r.readyState === 4 && !done) {
                if (!r.status && location.protocol === 'file:' ||
                        (r.status >= 200 && r.status < 300) ||
                        r.status === 304 ||
                        navigator.userAgent.indexOf('Safari') > -1 && typeof r.status === 'undefined') {
                    fnSuccess(r.responseText);
                }
                else {
                    fnFailure();
                }
                document.documentElement.style.cursor = '';
                r = null; // avoid memory leaks
            }
        };
        r.send('');
    };

    var sanitize = function (text) {
        text = text.replace(regExp.REDUNDANT_COMPONENTS, '');
        text = text.replace(regExp.REDUNDANT_WHITESPACE, '$1');
        text = text.replace(regExp.MORE_WHITESPACE, ' ');
        text = text.replace(regExp.FINAL_SEMICOLONS, '}'); // optional final semicolons
        return text;
    };

    var objects = {

        mediaQueryList: function (s) {
            var o = {};
            var idx = s.indexOf('{');
            var lt = s.substring(0, idx);
            s = s.substring(idx + 1, s.length - 1);
            var mqs = [], rs = [];

            // add media queries
            var qts = lt.toLowerCase().substring(7).split(',');
            for (var i = 0; i < qts.length; i++) { // parse each media query
                mqs[mqs.length] = objects.mediaQuery(qts[i], o);
            }

            // add rule sets
            var rts = s.match(regExp.BLOCKS_INSIDE);
            if (rts !== null) {
                for (i = 0; i < rts.length; i++) {
                    rs[rs.length] = objects.rule(rts[i], o);
                }
            }

            o.getMediaQueries = function () {
                return mqs;
            };
            o.getRules = function () {
                return rs;
            };
            o.getListText = function () {
                return lt;
            };
            o.getCssText = function () {
                return s;
            };
            return o;
        },

        mediaQuery: function (s, mql) {
            s = s || '';
            var not = false, type;
            var exp = [];
            var valid = true;
            var tokens = s.match(regExp.NOT_WHITESPACE);
            for (var i = 0; i < tokens.length; i++) {
                var token = tokens[i];
                if (!type && (token === 'not' || token === 'only')) { // 'not' and 'only' keywords
                    // keyword 'only' does nothing, as if it was not present
                    if (token === 'not') {
                        not = true;
                    }
                }
                else if (!type) { // media type
                    type = token;
                }
                else if (token.charAt(0) === '(') { // media feature expression
                    var pair = token.substring(1, token.length - 1).split(':');
                    exp[exp.length] = {
                        mediaFeature: pair[0],
                        value: pair[1] || null
                    };
                }
            }

            return {
                getList: function () {
                    return mql || null;
                },
                getValid: function () {
                    return valid;
                },
                getNot: function () {
                    return not;
                },
                getMediaType: function () {
                    return type;
                },
                getExpressions: function () {
                    return exp;
                }
            };
        },

        rule: function (s, mql) {
            var o = {};
            var idx = s.indexOf('{');
            var st = s.substring(0, idx);
            var ss = st.split(',');
            var ds = [];
            var dts = s.substring(idx + 1, s.length - 1).split(';');
            for (var i = 0; i < dts.length; i++) {
                ds[ds.length] = objects.declaration(dts[i], o);
            }

            o.getMediaQueryList = function () {
                return mql || null;
            };
            o.getSelectors = function () {
                return ss;
            };
            o.getSelectorText = function () {
                return st;
            };
            o.getDeclarations = function () {
                return ds;
            };
            o.getPropertyValue = function (n) {
                for (var i = 0; i < ds.length; i++) {
                    if (ds[i].getProperty() === n) {
                        return ds[i].getValue();
                    }
                }
                return null;
            };
            return o;
        },

        declaration: function (s, r) {
            var idx = s.indexOf(':');
            var p = s.substring(0, idx);
            var v = s.substring(idx + 1);
            return {
                getRule: function () {
                    return r || null;
                },
                getProperty: function () {
                    return p;
                },
                getValue: function () {
                    return v;
                }
            };
        }
    };

    var parseText = function (el) {
        if (typeof el.cssHelperText !== 'string') {
            return;
        }
        var o = {
            mediaQueryLists: [],
            rules: [],
            selectors: {},
            declarations: [],
            properties: {}
        };

        // parse blocks and collect media query lists and rules
        var mqls = o.mediaQueryLists;
        var ors = o.rules;
        var blocks = el.cssHelperText.match(regExp.BLOCKS);
        if (blocks !== null) {
            for (var i = 0; i < blocks.length; i++) {
                if (blocks[i].substring(0, 7) === '@media ') { // media query (list)
                    mqls[mqls.length] = objects.mediaQueryList(blocks[i]);
                    ors = o.rules = ors.concat(mqls[mqls.length - 1].getRules());
                }
                else { // regular rule set, page context (@page) or font description (@font-face)
                    ors[ors.length] = objects.rule(blocks[i]);
                }
            }
        }

        // collect selectors
        var oss = o.selectors;
        var collectSelectors = function (r) {
            var ss = r.getSelectors();
            for (var i = 0; i < ss.length; i++) {
                var n = ss[i];
                if (!oss[n]) {
                    oss[n] = [];
                }
                oss[n][oss[n].length] = r;
            }
        };
        for (i = 0; i < ors.length; i++) {
            collectSelectors(ors[i]);
        }

        // collect declarations
        var ods = o.declarations;
        for (i = 0; i < ors.length; i++) {
            ods = o.declarations = ods.concat(ors[i].getDeclarations());
        }

        // collect properties
        var ops = o.properties;
        for (i = 0; i < ods.length; i++) {
            var n = ods[i].getProperty();
            if (!ops[n]) {
                ops[n] = [];
            }
            ops[n][ops[n].length] = ods[i];
        }

        el.cssHelperParsed = o;
        parsed[parsed.length] = el;
        return o;
    };

    var parseEmbedded = function (el, s) {
        el.cssHelperText = sanitize(s || el.innerHTML); // bug in IE, where innerHTML gives us parsed css instead of raw literal
        return parseText(el);
    };

    var parse = function () {
        parsing = true;
        parsed = [];
        var linked = [];
        var finish = function () {
            for (var i = 0; i < linked.length; i++) {
                parseText(linked[i]);
            }
            var styles = document.getElementsByTagName('style');
            for (i = 0; i < styles.length; i++) {
                parseEmbedded(styles[i]);
            }
            parsing = false;
            ready();
        };
        var links = document.getElementsByTagName('link');
        for (var i = 0; i < links.length; i++) {
            var link = links[i];
            if (link.getAttribute('rel').indexOf('style') > -1 && link.href && link.href.length !== 0 && !link.disabled) {
                linked[linked.length] = link;
            }
        }
        if (linked.length > 0) {
            var c = 0;
            var checkForFinish = function () {
                c++;
                if (c === linked.length) { // parse in right order, so after last link is read
                    finish();
                }
            };
            var processLink = function (link) {
                var href = link.href;
                requestText(href, function (text) {
                    // fix url's
                    text = sanitize(text).replace(regExp.RELATIVE_URLS, 'url(' + href.substring(0, href.lastIndexOf('/')) + '/$1)');
                    link.cssHelperText = text;
                    checkForFinish();
                }, checkForFinish);
            };
            for (i = 0; i < linked.length; i++) {
                processLink(linked[i]);
            }
        }
        else {
            finish();
        }
    };

    var types = {
        mediaQueryLists: 'array',
        rules: 'array',
        selectors: 'object',
        declarations: 'array',
        properties: 'object'
    };

    var collections = {
        mediaQueryLists: null,
        rules: null,
        selectors: null,
        declarations: null,
        properties: null
    };

    var addToCollection = function (name, v) {
        if (collections[name] !== null) {
            if (types[name] === 'array') {
                return (collections[name] = collections[name].concat(v));
            }
            else {
                var c = collections[name];
                for (var n in v) {
                    if (v.hasOwnProperty(n)) {
                        if (!c[n]) {
                            c[n] = v[n];
                        }
                        else {
                            c[n] = c[n].concat(v[n]);
                        }
                    }
                }
                return c;
            }
        }
    };

    var collect = function (name) {
        collections[name] = (types[name] === 'array') ? [] : {};
        for (var i = 0; i < parsed.length; i++) {
            addToCollection(name, parsed[i].cssHelperParsed[name]);
        }
        return collections[name];
    };

    // timer for broadcasting added elements
    domReady(function () {
        var els = document.body.getElementsByTagName('*');
        for (var i = 0; i < els.length; i++) {
            els[i].checkedByCssHelper = true;
        }

        if (document.implementation.hasFeature('MutationEvents', '2.0') || window.MutationEvent) {
            document.body.addEventListener('DOMNodeInserted', function (e) {
                var el = e.target;
                if (el.nodeType === 1) {
                    broadcast('DOMElementInserted', el);
                    el.checkedByCssHelper = true;
                }
            }, false);
        }
        else {
            setInterval(function () {
                var els = document.body.getElementsByTagName('*');
                for (var i = 0; i < els.length; i++) {
                    if (!els[i].checkedByCssHelper) {
                        broadcast('DOMElementInserted', els[i]);
                        els[i].checkedByCssHelper = true;
                    }
                }
            }, 1000);
        }
    });

    // viewport size
    var getViewportSize = function (d) {
        if (typeof window.innerWidth != 'undefined') {
            return window["inner" + d];
        }
        else if (typeof document.documentElement != 'undefined'
                && typeof document.documentElement.clientWidth != 'undefined'
                && document.documentElement.clientWidth != 0) {
            return document.documentElement["client" + d];
        }
    };

    // public static functions
    return {
        addStyle: function (s, process) {
            var el;
                    if (null !== document.getElementById('css-mediaqueries-js')) {
                        el = document.getElementById('css-mediaqueries-js');
                    }
                    else {
                        el = document.createElement('style');
                        el.setAttribute('type', 'text/css');
                        el.setAttribute('id', 'css-mediaqueries-js');
                        document.getElementsByTagName('head')[0].appendChild(el);
                    }
                    if (el.styleSheet) { // IE
                        el.styleSheet.cssText += s;
                    }
                    else {
                        el.appendChild(document.createTextNode(s));
                    }
            el.addedWithCssHelper = true;
            if (typeof process === 'undefined' || process === true) {
                cssHelper.parsed(function (parsed) {
                    var o = parseEmbedded(el, s);
                    for (var n in o) {
                        if (o.hasOwnProperty(n)) {
                            addToCollection(n, o[n]);
                        }
                    }
                    broadcast('newStyleParsed', el);
                });
            }
            else {
                el.parsingDisallowed = true;
            }
            return el;
        },

        removeStyle: function (el) {
            if (el.parentNode)
                        return el.parentNode.removeChild(el);
        },

        parsed: function (fn) {
            if (parsing) {
                wait(fn);
            }
            else {
                if (typeof parsed !== 'undefined') {
                    if (typeof fn === 'function') {
                        fn(parsed);
                    }
                }
                else {
                    wait(fn);
                    parse();
                }
            }
        },

        mediaQueryLists: function (fn) {
            cssHelper.parsed(function (parsed) {
                fn(collections.mediaQueryLists || collect('mediaQueryLists'));
            });
        },

        rules: function (fn) {
            cssHelper.parsed(function (parsed) {
                fn(collections.rules || collect('rules'));
            });
        },

        selectors: function (fn) {
            cssHelper.parsed(function (parsed) {
                fn(collections.selectors || collect('selectors'));
            });
        },

        declarations: function (fn) {
            cssHelper.parsed(function (parsed) {
                fn(collections.declarations || collect('declarations'));
            });
        },

        properties: function (fn) {
            cssHelper.parsed(function (parsed) {
                fn(collections.properties || collect('properties'));
            });
        },

        broadcast: broadcast,

        addListener: function (n, fn) { // in case n is 'styleadd': added function is called everytime style is added and parsed
            if (typeof fn === 'function') {
                if (!events[n]) {
                    events[n] = {
                        listeners: []
                    };
                }
                events[n].listeners[events[n].listeners.length] = fn;
            }
        },

        removeListener: function (n, fn) {
            if (typeof fn === 'function' && events[n]) {
                var ls = events[n].listeners;
                for (var i = 0; i < ls.length; i++) {
                    if (ls[i] === fn) {
                        ls.splice(i, 1);
                        i -= 1;
                    }
                }
            }
        },

        getViewportWidth: function () {
            return getViewportSize("Width");
        },

        getViewportHeight: function () {
            return getViewportSize("Height");
        }
    };
}();



// function to test and apply parsed media queries against browser capabilities
domReady(function enableCssMediaQueries() {
    var meter;

    var regExp = {
        LENGTH_UNIT: /[0-9]+(em|ex|px|in|cm|mm|pt|pc)$/,
        RESOLUTION_UNIT: /[0-9]+(dpi|dpcm)$/,
        ASPECT_RATIO: /^[0-9]+\/[0-9]+$/,
        ABSOLUTE_VALUE: /^[0-9]*(\.[0-9]+)*$/
    };

    var styles = [];

    var nativeSupport = function () {
        // check support for media queries
        var id = 'css3-mediaqueries-test';
        var el = document.createElement('div');
        el.id = id;
        var style = cssHelper.addStyle('@media all and (width) { #' + id +
            ' { width: 1px !important; } }', false); // false means don't parse this temp style
        document.body.appendChild(el);
        var ret = el.offsetWidth === 1;
        style.parentNode.removeChild(style);
        el.parentNode.removeChild(el);
        nativeSupport = function () {
            return ret;
        };
        return ret;
    };

    var createMeter = function () { // create measuring element
        meter = document.createElement('div');
        meter.style.cssText = 'position:absolute;top:-9999em;left:-9999em;' +
            'margin:0;border:none;padding:0;width:1em;font-size:1em;'; // cssText is needed for IE, works for the others
        document.body.appendChild(meter);
        // meter must have browser default font size of 16px
        if (meter.offsetWidth !== 16) {
            meter.style.fontSize = 16 / meter.offsetWidth + 'em';
        }
        meter.style.width = '';
    };

    var measure = function (value) {
        meter.style.width = value;
        var amount = meter.offsetWidth;
        meter.style.width = '';
        return amount;
    };

    var testMediaFeature = function (feature, value) {
        // non-testable features: monochrome|min-monochrome|max-monochrome|scan|grid
        var l = feature.length;
        var min = (feature.substring(0, 4) === 'min-');
        var max = (!min && feature.substring(0, 4) === 'max-');

        if (value !== null) { // determine value type and parse to usable amount
            var valueType;
            var amount;
            if (regExp.LENGTH_UNIT.exec(value)) {
                valueType = 'length';
                amount = measure(value);
            }
            else if (regExp.RESOLUTION_UNIT.exec(value)) {
                valueType = 'resolution';
                amount = parseInt(value, 10);
                var unit = value.substring((amount + '').length);
            }
            else if (regExp.ASPECT_RATIO.exec(value)) {
                valueType = 'aspect-ratio';
                amount = value.split('/');
            }
            else if (regExp.ABSOLUTE_VALUE) {
                valueType = 'absolute';
                amount = value;
            }
            else {
                valueType = 'unknown';
            }
        }

        var width, height;
        if ('device-width' === feature.substring(l - 12, l)) { // screen width
            width = screen.width;
            if (value !== null) {
                if (valueType === 'length') {
                    return ((min && width >= amount) || (max && width < amount) || (!min && !max && width === amount));
                }
                else {
                    return false;
                }
            }
            else { // test width without value
                return width > 0;
            }
        }
        else if ('device-height' === feature.substring(l - 13, l)) { // screen height
            height = screen.height;
            if (value !== null) {
                if (valueType === 'length') {
                    return ((min && height >= amount) || (max && height < amount) || (!min && !max && height === amount));
                }
                else {
                    return false;
                }
            }
            else { // test height without value
                return height > 0;
            }
        }
        else if ('width' === feature.substring(l - 5, l)) { // viewport width
            width = document.documentElement.clientWidth || document.body.clientWidth; // the latter for IE quirks mode
            if (value !== null) {
                if (valueType === 'length') {
                    return ((min && width >= amount) || (max && width < amount) || (!min && !max && width === amount));
                }
                else {
                    return false;
                }
            }
            else { // test width without value
                return width > 0;
            }
        }
        else if ('height' === feature.substring(l - 6, l)) { // viewport height
            height = document.documentElement.clientHeight || document.body.clientHeight; // the latter for IE quirks mode
            if (value !== null) {
                if (valueType === 'length') {
                    return ((min && height >= amount) || (max && height < amount) || (!min && !max && height === amount));
                }
                else {
                    return false;
                }
            }
            else { // test height without value
                return height > 0;
            }
        }
        else if ('orientation' === feature.substring(l - 11, l)) { // orientation

            width = document.documentElement.clientWidth || document.body.clientWidth; // the latter for IE quirks mode
            height = document.documentElement.clientHeight || document.body.clientHeight; // the latter for IE quirks mode

            if (valueType === 'absolute') {
                return (amount === 'portrait') ? (width <= height) : (width > height);
            }
            else {
                return false;
            }
        }
        else if ('aspect-ratio' === feature.substring(l - 12, l)) { // window aspect ratio
            width = document.documentElement.clientWidth || document.body.clientWidth; // the latter for IE quirks mode
            height = document.documentElement.clientHeight || document.body.clientHeight; // the latter for IE quirks mode

            var curRatio = width / height;
            var ratio = amount[1] / amount[0];

            if (valueType === 'aspect-ratio') {
                return ((min && curRatio >= ratio) || (max && curRatio < ratio) || (!min && !max && curRatio === ratio));
            }
            else {
                return false;
            }
        }
        else if ('device-aspect-ratio' === feature.substring(l - 19, l)) { // screen aspect ratio
            return valueType === 'aspect-ratio' && screen.width * amount[1] === screen.height * amount[0];
        }
        else if ('color-index' === feature.substring(l - 11, l)) { // number of colors
            var colors = Math.pow(2, screen.colorDepth);
            if (value !== null) {
                if (valueType === 'absolute') {
                    return ((min && colors >= amount) || (max && colors < amount) || (!min && !max && colors === amount));
                }
                else {
                    return false;
                }
            }
            else { // test height without value
                return colors > 0;
            }
        }
        else if ('color' === feature.substring(l - 5, l)) { // bits per color component
            var color = screen.colorDepth;
            if (value !== null) {
                if (valueType === 'absolute') {
                    return ((min && color >= amount) || (max && color < amount) || (!min && !max && color === amount));
                }
                else {
                    return false;
                }
            }
            else { // test height without value
                return color > 0;
            }
        }
        else if ('resolution' === feature.substring(l - 10, l)) {
            var res;
            if (unit === 'dpcm') {
                res = measure('1cm');
            }
            else {
                res = measure('1in');
            }
            if (value !== null) {
                if (valueType === 'resolution') {
                    return ((min && res >= amount) || (max && res < amount) || (!min && !max && res === amount));
                }
                else {
                    return false;
                }
            }
            else { // test height without value
                return res > 0;
            }
        }
        else {
            return false;
        }
    };

    var testMediaQuery = function (mq) {
        var test = mq.getValid();
        var expressions = mq.getExpressions();
        var l = expressions.length;
        if (l > 0) {
            for (var i = 0; i < l && test; i++) {
                test = testMediaFeature(expressions[i].mediaFeature, expressions[i].value);
            }
            var not = mq.getNot();
            return (test && !not || not && !test);
        }
    };

    var testMediaQueryList = function (mql) {
        var mqs = mql.getMediaQueries();
        var t = {};
        for (var i = 0; i < mqs.length; i++) {
            if (testMediaQuery(mqs[i])) {
                t[mqs[i].getMediaType()] = true;
            }
        }
        var s = [], c = 0;
        for (var n in t) {
            if (t.hasOwnProperty(n)) {
                if (c > 0) {
                    s[c++] = ',';
                }
                s[c++] = n;
            }
        }
        if (s.length > 0) {
            styles[styles.length] = cssHelper.addStyle('@media ' + s.join('') + '{' + mql.getCssText() + '}', false);
        }
    };

    var testMediaQueryLists = function (mqls) {
        for (var i = 0; i < mqls.length; i++) {
            testMediaQueryList(mqls[i]);
        }
        if (ua.ie) {
            // force repaint in IE
            document.documentElement.style.display = 'block';
            setTimeout(function () {
                document.documentElement.style.display = '';
            }, 0);
            // delay broadcast somewhat for IE
            setTimeout(function () {
                cssHelper.broadcast('cssMediaQueriesTested');
            }, 100);
        }
        else {
            cssHelper.broadcast('cssMediaQueriesTested');
        }
    };

    var test = function () {
        for (var i = 0; i < styles.length; i++) {
            cssHelper.removeStyle(styles[i]);
        }
        styles = [];
        cssHelper.mediaQueryLists(testMediaQueryLists);
    };

    var scrollbarWidth = 0;
    var checkForResize = function () {
        var cvpw = cssHelper.getViewportWidth();
        var cvph = cssHelper.getViewportHeight();

        // determine scrollbar width in IE, see resizeHandler
        if (ua.ie) {
            var el = document.createElement('div');
            el.style.width = '100px';
            el.style.height = '100px';
            el.style.position = 'absolute';
            el.style.top = '-9999em';
            el.style.overflow = 'scroll';
            document.body.appendChild(el);
            scrollbarWidth = el.offsetWidth - el.clientWidth;
            document.body.removeChild(el);
        }

        var timer;
        var resizeHandler = function () {
            var vpw = cssHelper.getViewportWidth();
            var vph = cssHelper.getViewportHeight();
            // check whether vp size has really changed, because IE also triggers resize event when body size changes
            // 20px allowance to accomodate short appearance of scrollbars in IE in some cases
            if (Math.abs(vpw - cvpw) > scrollbarWidth || Math.abs(vph - cvph) > scrollbarWidth) {
                cvpw = vpw;
                cvph = vph;
                clearTimeout(timer);
                timer = setTimeout(function () {
                    if (!nativeSupport()) {
                        test();
                    }
                    else {
                        cssHelper.broadcast('cssMediaQueriesTested');
                    }
                }, 500);
            }
        };

        window.onresize = function () {
            var x = window.onresize || function () {}; // save original
            return function () {
                x();
                resizeHandler();
            };
        }();
    };

    // prevent jumping of layout by hiding everything before painting <body>
    var docEl = document.documentElement;
    docEl.style.marginLeft = '-32767px';

    // make sure it comes back after a while
    setTimeout(function () {
        docEl.style.marginTop = '';
    }, 20000);

    return function () {
        if (!nativeSupport()) { // if browser doesn't support media queries
            cssHelper.addListener('newStyleParsed', function (el) {
                testMediaQueryLists(el.cssHelperParsed.mediaQueryLists);
            });
            // return visibility after media queries are tested
            cssHelper.addListener('cssMediaQueriesTested', function () {
                // force repaint in IE by changing width
                if (ua.ie) {
                    docEl.style.width = '1px';
                }
                setTimeout(function () {
                    docEl.style.width = ''; // undo width
                    docEl.style.marginLeft = ''; // undo hide
                }, 0);
                // remove this listener to prevent following execution
                cssHelper.removeListener('cssMediaQueriesTested', arguments.callee);
            });
            createMeter();
            test();
        }
        else {
            docEl.style.marginLeft = ''; // undo visibility hidden
        }
        checkForResize();
    };
}());


// bonus: hotfix for IE6 SP1 (bug KB823727)
try {
    document.execCommand("BackgroundImageCache", false, true);
} catch (e) {}





(function(a){a.fn.checkedPolyfill=function(c){function b(){var f=a('<style type="text/css"> #checkedPolyfill-test:checked { margin-left: 123456px; display: none; } </style>}'),g=a('<input type="checkbox" checked id="checkedPolyfill-test" />'),e;a("head").append(f);a("body").append(g);if(g.css("margin-left")==="123456px"){e=true}else{e=false}f.remove();g.remove();return e}if(b()){return false}function d(f){var e=a('label[for="'+f.attr("id")+'"]');if(f.prop("checked")){f.addClass("checked");e.addClass("checked")}else{f.removeClass("checked");e.removeClass("checked")}return f}return this.each(function(){var e=a(this);if(e.prop("type")==="radio"){a('input[name="'+e.prop("name")+'"]').change(function(){d(e)})}else{if(e.prop("type")==="checkbox"){e.change(function(){d(e)})}}d(e)})}})(jQuery);





/*! viewportSize | Author: Tyson Matanich, 2013 | License: MIT */
(function(n){n.viewportSize={},n.viewportSize.getHeight=function(){return t("Height")},n.viewportSize.getWidth=function(){return t("Width")};var t=function(t){var f,o=t.toLowerCase(),e=n.document,i=e.documentElement,r,u;return n["inner"+t]===undefined?f=i["client"+t]:n["inner"+t]!=i["client"+t]?(r=e.createElement("body"),r.id="vpw-test-b",r.style.cssText="overflow:scroll",u=e.createElement("div"),u.id="vpw-test-d",u.style.cssText="position:absolute;top:-1000px",u.innerHTML="<style>@media("+o+":"+i["client"+t]+"px){body#vpw-test-b div#vpw-test-d{"+o+":7px!important}}<\/style>",r.appendChild(u),i.insertBefore(r,e.head),f=u["offset"+t]==7?i["client"+t]:n["inner"+t],i.removeChild(r)):f=n["inner"+t],f}})(this);





(function($,sr){

  // debouncing function from John Hann
  // http://unscriptable.com/index.php/2009/03/20/debouncing-javascript-methods/
  var debounce = function (func, threshold, execAsap) {
      var timeout;

      return function debounced () {
          var obj = this, args = arguments;
          function delayed () {
              if (!execAsap)
                  func.apply(obj, args);
              timeout = null;
          };

          if (timeout)
              clearTimeout(timeout);
          else if (execAsap)
              func.apply(obj, args);

          timeout = setTimeout(delayed, threshold || 100);
      };
  }
  // smartresize 
  jQuery.fn[sr] = function(fn){  return fn ? this.bind('resize', debounce(fn)) : this.trigger(sr); };

})(jQuery,'smartresize');





// Simple plugin that sets elements with the specified class to the same height
$.fn.setAllToMaxHeight = function() {
    return this.height(
        Math.max.apply(
            this, $.map(
                this , function(e) {
                    return $(e).height();
                }
            )
        )
    );
};





/*!
 * hoverIntent r7 // 2013.03.11 // jQuery 1.9.1+
 * http://cherne.net/brian/resources/jquery.hoverIntent.html
 *
 * You may use hoverIntent under the terms of the MIT license.
 * Copyright 2007, 2013 Brian Cherne
 */
(function(e){e.fn.hoverIntent=function(t,n,r){var i={interval:100,sensitivity:7,timeout:0};if(typeof t==="object"){i=e.extend(i,t)}else if(e.isFunction(n)){i=e.extend(i,{over:t,out:n,selector:r})}else{i=e.extend(i,{over:t,out:t,selector:n})}var s,o,u,a;var f=function(e){s=e.pageX;o=e.pageY};var l=function(t,n){n.hoverIntent_t=clearTimeout(n.hoverIntent_t);if(Math.abs(u-s)+Math.abs(a-o)<i.sensitivity){e(n).off("mousemove.hoverIntent",f);n.hoverIntent_s=1;return i.over.apply(n,[t])}else{u=s;a=o;n.hoverIntent_t=setTimeout(function(){l(t,n)},i.interval)}};var c=function(e,t){t.hoverIntent_t=clearTimeout(t.hoverIntent_t);t.hoverIntent_s=0;return i.out.apply(t,[e])};var h=function(t){var n=jQuery.extend({},t);var r=this;if(r.hoverIntent_t){r.hoverIntent_t=clearTimeout(r.hoverIntent_t)}if(t.type=="mouseenter"){u=n.pageX;a=n.pageY;e(r).on("mousemove.hoverIntent",f);if(r.hoverIntent_s!=1){r.hoverIntent_t=setTimeout(function(){l(n,r)},i.interval)}}else{e(r).off("mousemove.hoverIntent",f);if(r.hoverIntent_s==1){r.hoverIntent_t=setTimeout(function(){c(n,r)},i.timeout)}}};return this.on({"mouseenter.hoverIntent":h,"mouseleave.hoverIntent":h},i.selector)}})(jQuery)





/*
 jQuery Simple Slider

 Copyright (c) 2012 James Smith (http://loopj.com)

 Licensed under the MIT license (http://mit-license.org/)
*/

var __slice = [].slice,
  __indexOf = [].indexOf || function(item) { for (var i = 0, l = this.length; i < l; i++) { if (i in this && this[i] === item) return i; } return -1; };

(function($, window) {
  var SimpleSlider;
  SimpleSlider = (function() {

    function SimpleSlider(input, options) {
      var ratio,
        _this = this;
      this.input = input;
      this.defaultOptions = {
        animate: true,
        snapMid: false,
        classPrefix: null,
        classSuffix: null,
        theme: null,
        highlight: false
      };
      this.settings = $.extend({}, this.defaultOptions, options);
      if (this.settings.theme) {
        this.settings.classSuffix = "-" + this.settings.theme;
      }
      this.input.hide();
      this.slider = $("<div>").addClass("slider" + (this.settings.classSuffix || "")).css({
        position: "relative",
        userSelect: "none",
        boxSizing: "border-box"
      }).insertBefore(this.input);
      if (this.input.attr("id")) {
        this.slider.attr("id", this.input.attr("id") + "-slider");
      }
      this.track = this.createDivElement("track").css({
        width: "100%"
      });
      if (this.settings.highlight) {
        this.highlightTrack = this.createDivElement("highlight-track").css({
          width: "0"
        });
      }
      this.dragger = this.createDivElement("dragger");
      this.slider.css({
        minHeight: this.dragger.outerHeight(),
        // marginLeft: this.dragger.outerWidth() / 2,
        // marginRight: this.dragger.outerWidth() / 2
        marginLeft: 0,
        marginRight: 0
      });
      this.track.css({
        marginTop: this.track.outerHeight() / -2
      });
      if (this.settings.highlight) {
        this.highlightTrack.css({
          marginTop: this.track.outerHeight() / -2
        });
      }
      this.dragger.css({
        // marginTop: this.dragger.outerHeight() / -2,
        marginLeft: this.dragger.outerWidth() / -2,
        marginTop: -11
      });
      this.track.mousedown(function(e) {
        return _this.trackEvent(e);
      });
      if (this.settings.highlight) {
        this.highlightTrack.mousedown(function(e) {
          return _this.trackEvent(e);
        });
      }
      this.dragger.mousedown(function(e) {
        if (e.which !== 1) {
          return;
        }
        _this.dragging = true;
        _this.dragger.addClass("dragging");
        _this.domDrag(e.pageX, e.pageY);
        return false;
      });
      $("body").mousemove(function(e) {
        if (_this.dragging) {
          _this.domDrag(e.pageX, e.pageY);
          return $("body").css({
            cursor: "pointer"
          });
        }
      }).mouseup(function(e) {
        if (_this.dragging) {
          _this.dragging = false;
          _this.dragger.removeClass("dragging");
          return $("body").css({
            cursor: "auto"
          });
        }
      });
      this.pagePos = 0;
      if (this.input.val() === "") {
        this.value = this.getRange().min;
        this.input.val(this.value);
      } else {
        this.value = this.nearestValidValue(this.input.val());
      }
      this.setSliderPositionFromValue(this.value);
      ratio = this.valueToRatio(this.value);
      this.input.trigger("slider:ready", {
        value: this.value,
        ratio: ratio,
        position: ratio * this.slider.outerWidth(),
        el: this.slider
      });
    }

    SimpleSlider.prototype.createDivElement = function(classname) {
      var item;
      item = $("<div>").addClass(classname).css({
        position: "absolute",
        top: "50%",
        userSelect: "none",
        cursor: "pointer"
      }).appendTo(this.slider);
      return item;
    };

    SimpleSlider.prototype.setRatio = function(ratio) {
      var value;
      ratio = Math.min(1, ratio);
      ratio = Math.max(0, ratio);
      value = this.ratioToValue(ratio);
      this.setSliderPositionFromValue(value);
      return this.valueChanged(value, ratio, "setRatio");
    };

    SimpleSlider.prototype.setValue = function(value) {
      var ratio;
      value = this.nearestValidValue(value);
      ratio = this.valueToRatio(value);
      this.setSliderPositionFromValue(value);
      return this.valueChanged(value, ratio, "setValue");
    };

    SimpleSlider.prototype.trackEvent = function(e) {
      if (e.which !== 1) {
        return;
      }
      this.domDrag(e.pageX, e.pageY, true);
      this.dragging = true;
      return false;
    };

    SimpleSlider.prototype.domDrag = function(pageX, pageY, animate) {
      var pagePos, ratio, value;
      if (animate == null) {
        animate = false;
      }
      pagePos = pageX - this.slider.offset().left;
      pagePos = Math.min(this.slider.outerWidth(), pagePos);
      pagePos = Math.max(0, pagePos);
      if (this.pagePos !== pagePos) {
        this.pagePos = pagePos;
        ratio = pagePos / this.slider.outerWidth();
        value = this.ratioToValue(ratio);
        this.valueChanged(value, ratio, "domDrag");
        if (this.settings.snap) {
          return this.setSliderPositionFromValue(value, animate);
        } else {
          return this.setSliderPosition(pagePos, animate);
        }
      }
    };

    SimpleSlider.prototype.setSliderPosition = function(position, animate) {
      if (animate == null) {
        animate = false;
      }
      if (animate && this.settings.animate) {
        this.dragger.animate({
          left: position
        }, 200);
        if (this.settings.highlight) {
          return this.highlightTrack.animate({
            width: position
          }, 200);
        }
      } else {
        this.dragger.css({
          left: position
        });
        if (this.settings.highlight) {
          return this.highlightTrack.css({
            width: position
          });
        }
      }
    };

    SimpleSlider.prototype.setSliderPositionFromValue = function(value, animate) {
      var ratio;
      if (animate == null) {
        animate = false;
      }
      ratio = this.valueToRatio(value);
      return this.setSliderPosition(ratio * this.slider.outerWidth(), animate);
    };

    SimpleSlider.prototype.getRange = function() {
      if (this.settings.allowedValues) {
        return {
          min: Math.min.apply(Math, this.settings.allowedValues),
          max: Math.max.apply(Math, this.settings.allowedValues)
        };
      } else if (this.settings.range) {
        return {
          min: parseFloat(this.settings.range[0]),
          max: parseFloat(this.settings.range[1])
        };
      } else {
        return {
          min: 0,
          max: 1
        };
      }
    };

    SimpleSlider.prototype.nearestValidValue = function(rawValue) {
      var closest, maxSteps, range, steps;
      range = this.getRange();
      rawValue = Math.min(range.max, rawValue);
      rawValue = Math.max(range.min, rawValue);
      if (this.settings.allowedValues) {
        closest = null;
        $.each(this.settings.allowedValues, function() {
          if (closest === null || Math.abs(this - rawValue) < Math.abs(closest - rawValue)) {
            return closest = this;
          }
        });
        return closest;
      } else if (this.settings.step) {
        maxSteps = (range.max - range.min) / this.settings.step;
        steps = Math.floor((rawValue - range.min) / this.settings.step);
        if ((rawValue - range.min) % this.settings.step > this.settings.step / 2 && steps < maxSteps) {
          steps += 1;
        }
        return steps * this.settings.step + range.min;
      } else {
        return rawValue;
      }
    };

    SimpleSlider.prototype.valueToRatio = function(value) {
      var allowedVal, closest, closestIdx, idx, range, _i, _len, _ref;
      if (this.settings.equalSteps) {
        _ref = this.settings.allowedValues;
        for (idx = _i = 0, _len = _ref.length; _i < _len; idx = ++_i) {
          allowedVal = _ref[idx];
          if (!(typeof closest !== "undefined" && closest !== null) || Math.abs(allowedVal - value) < Math.abs(closest - value)) {
            closest = allowedVal;
            closestIdx = idx;
          }
        }
        if (this.settings.snapMid) {
          return (closestIdx + 0.5) / this.settings.allowedValues.length;
        } else {
          return closestIdx / (this.settings.allowedValues.length - 1);
        }
      } else {
        range = this.getRange();
        return (value - range.min) / (range.max - range.min);
      }
    };

    SimpleSlider.prototype.ratioToValue = function(ratio) {
      var idx, range, rawValue, step, steps;
      if (this.settings.equalSteps) {
        steps = this.settings.allowedValues.length;
        step = Math.round(ratio * steps - 0.5);
        idx = Math.min(step, this.settings.allowedValues.length - 1);
        return this.settings.allowedValues[idx];
      } else {
        range = this.getRange();
        rawValue = ratio * (range.max - range.min) + range.min;
        return this.nearestValidValue(rawValue);
      }
    };

    SimpleSlider.prototype.valueChanged = function(value, ratio, trigger) {
      var eventData;
      if (value.toString() === this.value.toString()) {
        return;
      }
      this.value = value;
      eventData = {
        value: value,
        ratio: ratio,
        position: ratio * this.slider.outerWidth(),
        trigger: trigger,
        el: this.slider
      };
      return this.input.val(value).trigger($.Event("change", eventData)).trigger("slider:changed", eventData);
    };

    return SimpleSlider;

  })();
  $.extend($.fn, {
    simpleSlider: function() {
      var params, publicMethods, settingsOrMethod;
      settingsOrMethod = arguments[0], params = 2 <= arguments.length ? __slice.call(arguments, 1) : [];
      publicMethods = ["setRatio", "setValue"];
      return $(this).each(function() {
        var obj, settings;
        if (settingsOrMethod && __indexOf.call(publicMethods, settingsOrMethod) >= 0) {
          obj = $(this).data("slider-object");
          return obj[settingsOrMethod].apply(obj, params);
        } else {
          settings = settingsOrMethod;
          return $(this).data("slider-object", new SimpleSlider($(this), settings));
        }
      });
    }
  });
  return $(function() {
    return $("[data-slider]").each(function() {
      var $el, allowedValues, settings, x;
      $el = $(this);
      settings = {};
      allowedValues = $el.data("slider-values");
      if (allowedValues) {
        settings.allowedValues = (function() {
          var _i, _len, _ref, _results;
          _ref = allowedValues.split(",");
          _results = [];
          for (_i = 0, _len = _ref.length; _i < _len; _i++) {
            x = _ref[_i];
            _results.push(parseFloat(x));
          }
          return _results;
        })();
      }
      if ($el.data("slider-range")) {
        settings.range = $el.data("slider-range").split(",");
      }
      if ($el.data("slider-step")) {
        settings.step = $el.data("slider-step");
      }
      settings.snap = $el.data("slider-snap");
      settings.equalSteps = $el.data("slider-equal-steps");
      if ($el.data("slider-theme")) {
        settings.theme = $el.data("slider-theme");
      }
      if ($el.attr("data-slider-highlight")) {
        settings.highlight = $el.data("slider-highlight");
      }
      if ($el.data("slider-animate") != null) {
        settings.animate = $el.data("slider-animate");
      }
      return $el.simpleSlider(settings);
    });
  });
})(this.jQuery || this.Zepto, this);
 
/**
 * jQuery Masonry v2.1.08
 * A dynamic layout plugin for jQuery
 * The flip-side of CSS Floats
 * http://masonry.desandro.com
 *
 * Licensed under the MIT license.
 * Copyright 2012 David DeSandro
 */
(function (e, t, n) { "use strict"; var r = t.event, i; r.special.smartresize = { setup: function () { t(this).bind("resize", r.special.smartresize.handler) }, teardown: function () { t(this).unbind("resize", r.special.smartresize.handler) }, handler: function (e, t) { var n = this, s = arguments; e.type = "smartresize", i && clearTimeout(i), i = setTimeout(function () { r.dispatch.apply(n, s) }, t === "execAsap" ? 0 : 100) } }, t.fn.smartresize = function (e) { return e ? this.bind("smartresize", e) : this.trigger("smartresize", ["execAsap"]) }, t.Mason = function (e, n) { this.element = t(n), this._create(e), this._init() }, t.Mason.settings = { isResizable: !0, isAnimated: !1, animationOptions: { queue: !1, duration: 500 }, gutterWidth: 0, isRTL: !1, isFitWidth: !1, containerStyle: { position: "relative" } }, t.Mason.prototype = { _filterFindBricks: function (e) { var t = this.options.itemSelector; return t ? e.filter(t).add(e.find(t)) : e }, _getBricks: function (e) { var t = this._filterFindBricks(e).css({ position: "absolute" }).addClass("masonry-brick"); return t }, _create: function (n) { this.options = t.extend(!0, {}, t.Mason.settings, n), this.styleQueue = []; var r = this.element[0].style; this.originalStyle = { height: r.height || "" }; var i = this.options.containerStyle; for (var s in i) this.originalStyle[s] = r[s] || ""; this.element.css(i), this.horizontalDirection = this.options.isRTL ? "right" : "left"; var o = this.element.css("padding-" + this.horizontalDirection), u = this.element.css("padding-top"); this.offset = { x: o ? parseInt(o, 10) : 0, y: u ? parseInt(u, 10) : 0 }, this.isFluid = this.options.columnWidth && typeof this.options.columnWidth == "function"; var a = this; setTimeout(function () { a.element.addClass("masonry") }, 0), this.options.isResizable && t(e).bind("smartresize.masonry", function () { a.resize() }), this.reloadItems() }, _init: function (e) { this._getColumns(), this._reLayout(e) }, option: function (e, n) { t.isPlainObject(e) && (this.options = t.extend(!0, this.options, e)) }, layout: function (e, t) { for (var n = 0, r = e.length; n < r; n++) this._placeBrick(e[n]); var i = {}; i.height = Math.max.apply(Math, this.colYs); if (this.options.isFitWidth) { var s = 0; n = this.cols; while (--n) { if (this.colYs[n] !== 0) break; s++ } i.width = (this.cols - s) * this.columnWidth - this.options.gutterWidth } this.styleQueue.push({ $el: this.element, style: i }); var o = this.isLaidOut ? this.options.isAnimated ? "animate" : "css" : "css", u = this.options.animationOptions, a; for (n = 0, r = this.styleQueue.length; n < r; n++) a = this.styleQueue[n], a.$el[o](a.style, u); this.styleQueue = [], t && t.call(e), this.isLaidOut = !0 }, _getColumns: function () { var e = this.options.isFitWidth ? this.element.parent() : this.element, t = e.width(); this.columnWidth = this.isFluid ? this.options.columnWidth(t) : this.options.columnWidth || this.$bricks.outerWidth(!0) || t, this.columnWidth += this.options.gutterWidth, this.cols = Math.floor((t + this.options.gutterWidth) / this.columnWidth), this.cols = Math.max(this.cols, 1) }, _placeBrick: function (e) { var n = t(e), r, i, s, o, u; r = Math.ceil(n.outerWidth(!0) / this.columnWidth), r = Math.min(r, this.cols); if (r === 1) s = this.colYs; else { i = this.cols + 1 - r, s = []; for (u = 0; u < i; u++) o = this.colYs.slice(u, u + r), s[u] = Math.max.apply(Math, o) } var a = Math.min.apply(Math, s), f = 0; for (var l = 0, c = s.length; l < c; l++) if (s[l] === a) { f = l; break } var h = { top: a + this.offset.y }; h[this.horizontalDirection] = this.columnWidth * f + this.offset.x, this.styleQueue.push({ $el: n, style: h }); var p = a + n.outerHeight(!0), d = this.cols + 1 - c; for (l = 0; l < d; l++) this.colYs[f + l] = p }, resize: function () { var e = this.cols; this._getColumns(), (this.isFluid || this.cols !== e) && this._reLayout() }, _reLayout: function (e) { var t = this.cols; this.colYs = []; while (t--) this.colYs.push(0); this.layout(this.$bricks, e) }, reloadItems: function () { this.$bricks = this._getBricks(this.element.children()) }, reload: function (e) { this.reloadItems(), this._init(e) }, appended: function (e, t, n) { if (t) { this._filterFindBricks(e).css({ top: this.element.height() }); var r = this; setTimeout(function () { r._appended(e, n) }, 1) } else this._appended(e, n) }, _appended: function (e, t) { var n = this._getBricks(e); this.$bricks = this.$bricks.add(n), this.layout(n, t) }, remove: function (e) { this.$bricks = this.$bricks.not(e), e.remove() }, destroy: function () { this.$bricks.removeClass("masonry-brick").each(function () { this.style.position = "", this.style.top = "", this.style.left = "" }); var n = this.element[0].style; for (var r in this.originalStyle) n[r] = this.originalStyle[r]; this.element.unbind(".masonry").removeClass("masonry").removeData("masonry"), t(e).unbind(".masonry") } }, t.fn.imagesLoaded = function (e) { function u() { e.call(n, r) } function a(e) { var n = e.target; n.src !== s && t.inArray(n, o) === -1 && (o.push(n), --i <= 0 && (setTimeout(u), r.unbind(".imagesLoaded", a))) } var n = this, r = n.find("img").add(n.filter("img")), i = r.length, s = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==", o = []; return i || u(), r.bind("load.imagesLoaded error.imagesLoaded", a).each(function () { var e = this.src; this.src = s, this.src = e }), n }; var s = function (t) { e.console && e.console.error(t) }; t.fn.masonry = function (e) { if (typeof e == "string") { var n = Array.prototype.slice.call(arguments, 1); this.each(function () { var r = t.data(this, "masonry"); if (!r) { s("cannot call methods on masonry prior to initialization; attempted to call method '" + e + "'"); return } if (!t.isFunction(r[e]) || e.charAt(0) === "_") { s("no such method '" + e + "' for masonry instance"); return } r[e].apply(r, n) }) } else this.each(function () { var n = t.data(this, "masonry"); n ? (n.option(e || {}), n._init()) : t.data(this, "masonry", new t.Mason(e, this)) }); return this } })(window, jQuery);

/*!
 * imagesLoaded PACKAGED v3.1.8
 * JavaScript is all like "You images are done yet or what?"
 * MIT License
 */

(function () { function e() { } function t(e, t) { for (var n = e.length; n--;) if (e[n].listener === t) return n; return -1 } function n(e) { return function () { return this[e].apply(this, arguments) } } var i = e.prototype, r = this, o = r.EventEmitter; i.getListeners = function (e) { var t, n, i = this._getEvents(); if ("object" == typeof e) { t = {}; for (n in i) i.hasOwnProperty(n) && e.test(n) && (t[n] = i[n]) } else t = i[e] || (i[e] = []); return t }, i.flattenListeners = function (e) { var t, n = []; for (t = 0; e.length > t; t += 1) n.push(e[t].listener); return n }, i.getListenersAsObject = function (e) { var t, n = this.getListeners(e); return n instanceof Array && (t = {}, t[e] = n), t || n }, i.addListener = function (e, n) { var i, r = this.getListenersAsObject(e), o = "object" == typeof n; for (i in r) r.hasOwnProperty(i) && -1 === t(r[i], n) && r[i].push(o ? n : { listener: n, once: !1 }); return this }, i.on = n("addListener"), i.addOnceListener = function (e, t) { return this.addListener(e, { listener: t, once: !0 }) }, i.once = n("addOnceListener"), i.defineEvent = function (e) { return this.getListeners(e), this }, i.defineEvents = function (e) { for (var t = 0; e.length > t; t += 1) this.defineEvent(e[t]); return this }, i.removeListener = function (e, n) { var i, r, o = this.getListenersAsObject(e); for (r in o) o.hasOwnProperty(r) && (i = t(o[r], n), -1 !== i && o[r].splice(i, 1)); return this }, i.off = n("removeListener"), i.addListeners = function (e, t) { return this.manipulateListeners(!1, e, t) }, i.removeListeners = function (e, t) { return this.manipulateListeners(!0, e, t) }, i.manipulateListeners = function (e, t, n) { var i, r, o = e ? this.removeListener : this.addListener, s = e ? this.removeListeners : this.addListeners; if ("object" != typeof t || t instanceof RegExp) for (i = n.length; i--;) o.call(this, t, n[i]); else for (i in t) t.hasOwnProperty(i) && (r = t[i]) && ("function" == typeof r ? o.call(this, i, r) : s.call(this, i, r)); return this }, i.removeEvent = function (e) { var t, n = typeof e, i = this._getEvents(); if ("string" === n) delete i[e]; else if ("object" === n) for (t in i) i.hasOwnProperty(t) && e.test(t) && delete i[t]; else delete this._events; return this }, i.removeAllListeners = n("removeEvent"), i.emitEvent = function (e, t) { var n, i, r, o, s = this.getListenersAsObject(e); for (r in s) if (s.hasOwnProperty(r)) for (i = s[r].length; i--;) n = s[r][i], n.once === !0 && this.removeListener(e, n.listener), o = n.listener.apply(this, t || []), o === this._getOnceReturnValue() && this.removeListener(e, n.listener); return this }, i.trigger = n("emitEvent"), i.emit = function (e) { var t = Array.prototype.slice.call(arguments, 1); return this.emitEvent(e, t) }, i.setOnceReturnValue = function (e) { return this._onceReturnValue = e, this }, i._getOnceReturnValue = function () { return this.hasOwnProperty("_onceReturnValue") ? this._onceReturnValue : !0 }, i._getEvents = function () { return this._events || (this._events = {}) }, e.noConflict = function () { return r.EventEmitter = o, e }, "function" == typeof define && define.amd ? define("eventEmitter/EventEmitter", [], function () { return e }) : "object" == typeof module && module.exports ? module.exports = e : this.EventEmitter = e }).call(this), function (e) { function t(t) { var n = e.event; return n.target = n.target || n.srcElement || t, n } var n = document.documentElement, i = function () { }; n.addEventListener ? i = function (e, t, n) { e.addEventListener(t, n, !1) } : n.attachEvent && (i = function (e, n, i) { e[n + i] = i.handleEvent ? function () { var n = t(e); i.handleEvent.call(i, n) } : function () { var n = t(e); i.call(e, n) }, e.attachEvent("on" + n, e[n + i]) }); var r = function () { }; n.removeEventListener ? r = function (e, t, n) { e.removeEventListener(t, n, !1) } : n.detachEvent && (r = function (e, t, n) { e.detachEvent("on" + t, e[t + n]); try { delete e[t + n] } catch (i) { e[t + n] = void 0 } }); var o = { bind: i, unbind: r }; "function" == typeof define && define.amd ? define("eventie/eventie", o) : e.eventie = o }(this), function (e, t) { "function" == typeof define && define.amd ? define(["eventEmitter/EventEmitter", "eventie/eventie"], function (n, i) { return t(e, n, i) }) : "object" == typeof exports ? module.exports = t(e, require("wolfy87-eventemitter"), require("eventie")) : e.imagesLoaded = t(e, e.EventEmitter, e.eventie) }(window, function (e, t, n) { function i(e, t) { for (var n in t) e[n] = t[n]; return e } function r(e) { return "[object Array]" === d.call(e) } function o(e) { var t = []; if (r(e)) t = e; else if ("number" == typeof e.length) for (var n = 0, i = e.length; i > n; n++) t.push(e[n]); else t.push(e); return t } function s(e, t, n) { if (!(this instanceof s)) return new s(e, t); "string" == typeof e && (e = document.querySelectorAll(e)), this.elements = o(e), this.options = i({}, this.options), "function" == typeof t ? n = t : i(this.options, t), n && this.on("always", n), this.getImages(), a && (this.jqDeferred = new a.Deferred); var r = this; setTimeout(function () { r.check() }) } function f(e) { this.img = e } function c(e) { this.src = e, v[e] = this } var a = e.jQuery, u = e.console, h = u !== void 0, d = Object.prototype.toString; s.prototype = new t, s.prototype.options = {}, s.prototype.getImages = function () { this.images = []; for (var e = 0, t = this.elements.length; t > e; e++) { var n = this.elements[e]; "IMG" === n.nodeName && this.addImage(n); var i = n.nodeType; if (i && (1 === i || 9 === i || 11 === i)) for (var r = n.querySelectorAll("img"), o = 0, s = r.length; s > o; o++) { var f = r[o]; this.addImage(f) } } }, s.prototype.addImage = function (e) { var t = new f(e); this.images.push(t) }, s.prototype.check = function () { function e(e, r) { return t.options.debug && h && u.log("confirm", e, r), t.progress(e), n++, n === i && t.complete(), !0 } var t = this, n = 0, i = this.images.length; if (this.hasAnyBroken = !1, !i) return this.complete(), void 0; for (var r = 0; i > r; r++) { var o = this.images[r]; o.on("confirm", e), o.check() } }, s.prototype.progress = function (e) { this.hasAnyBroken = this.hasAnyBroken || !e.isLoaded; var t = this; setTimeout(function () { t.emit("progress", t, e), t.jqDeferred && t.jqDeferred.notify && t.jqDeferred.notify(t, e) }) }, s.prototype.complete = function () { var e = this.hasAnyBroken ? "fail" : "done"; this.isComplete = !0; var t = this; setTimeout(function () { if (t.emit(e, t), t.emit("always", t), t.jqDeferred) { var n = t.hasAnyBroken ? "reject" : "resolve"; t.jqDeferred[n](t) } }) }, a && (a.fn.imagesLoaded = function (e, t) { var n = new s(this, e, t); return n.jqDeferred.promise(a(this)) }), f.prototype = new t, f.prototype.check = function () { var e = v[this.img.src] || new c(this.img.src); if (e.isConfirmed) return this.confirm(e.isLoaded, "cached was confirmed"), void 0; if (this.img.complete && void 0 !== this.img.naturalWidth) return this.confirm(0 !== this.img.naturalWidth, "naturalWidth"), void 0; var t = this; e.on("confirm", function (e, n) { return t.confirm(e.isLoaded, n), !0 }), e.check() }, f.prototype.confirm = function (e, t) { this.isLoaded = e, this.emit("confirm", this, t) }; var v = {}; return c.prototype = new t, c.prototype.check = function () { if (!this.isChecked) { var e = new Image; n.bind(e, "load", this), n.bind(e, "error", this), e.src = this.src, this.isChecked = !0 } }, c.prototype.handleEvent = function (e) { var t = "on" + e.type; this[t] && this[t](e) }, c.prototype.onload = function (e) { this.confirm(!0, "onload"), this.unbindProxyEvents(e) }, c.prototype.onerror = function (e) { this.confirm(!1, "onerror"), this.unbindProxyEvents(e) }, c.prototype.confirm = function (e, t) { this.isConfirmed = !0, this.isLoaded = e, this.emit("confirm", this, t) }, c.prototype.unbindProxyEvents = function (e) { n.unbind(e.target, "load", this), n.unbind(e.target, "error", this) }, s });