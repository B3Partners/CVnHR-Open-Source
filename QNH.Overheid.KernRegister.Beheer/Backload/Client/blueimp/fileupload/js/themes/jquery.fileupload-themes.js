/*
 * jQuery File Upload User Interface Plugin 9.6.1
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/* jshint nomen:false */
/* global define, require, window */

(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // Register as an anonymous AMD module:
        define([
            'jquery',
            '../jquery.fileupload'
        ], factory);
    } else if (typeof exports === 'object') {
        // Node/CommonJS:
        factory(
            require('jquery'),
            require('../jquery.fileupload')
        );
    } else {
        // Browser globals:
        factory(
            window.jQuery
        );
    }
}(function ($) {
    'use strict';

    // The UI version extends the file upload widget
    // and adds complete user interface interaction:
    $.widget('blueimp.fileupload', $.blueimp.fileupload, {
        getUploadButton: function() {
            var uploadButton = $('<button/>')
            .addClass('btn')
            .prop('disabled', true)
            .text('Processing...')
            .on('click', function () {
                var $this = $(this),
                    data = $this.data();
                $this
                    .off('click')
                    .text('Abort')
                    .on('click', function () {
                        $this.remove();
                        data.abort();
                    });
                data.submit().always(function () {
                    $this.remove();
                });
            });
            return uploadButton;
        },

        // Bind event handler to the client plugin events
        initTheme: function (theme) {
            var $this = this.element;
            var low = theme.toLowerCase();
            if ($this) {
                if (low == "basic")
                    this._initBasicTheme($this);
                else if (low == "basicplus")
                    this._initBasicPlusTheme($this);
                else if ((low == "basicplusui") || (low == "jqueryui"))
                    this._initBasicPlusUITheme($this);
                return $this;
            }
            return null;
        },

        // Bind event handlers in Basic Plus Theme
        _initBasicTheme: function($this) {
            $this.on('fileuploaddone', function (e, data) {
                $.each(data.result.files, function (index, file) {
                    $('<p/>').text(file.name).appendTo('#files');
                });
            })
            .on('fileuploadprogressall', function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#progress .progress-bar').css(
                    'width',
                    progress + '%'
                );
            })
            .prop('disabled', !$.support.fileInput)
            .parent().addClass($.support.fileInput ? undefined : 'disabled')
        },

        // Bind event handlers in Basic Plus Theme
        _initBasicPlusTheme: function($this) {
            $this.on('fileuploadadd', function (e, data) {
                var $widget = $(e.target).data('blueimp-fileupload');
                var uploadButton = $widget.getUploadButton();
                data.context = $('<div/>').appendTo('#files');
                $.each(data.files, function (index, file) {
                    var node = $('<p/>')
                            .append($('<span/>').text(file.name));
                    if (!index) {
                        node
                            .append('<br>')
                            .append(uploadButton.clone(true).data(data));
                    }
                    node.appendTo(data.context);
                });
            })

            .on('fileuploadprocessalways', function (e, data) {
                var index = data.index,
                    file = data.files[index],
                    node = $(data.context.children()[index]);
                if (file.preview) {
                    node
                        .prepend('<br>')
                        .prepend(file.preview);
                }
                if (file.error) {
                    node
                        .append('<br>')
                        .append(file.error);
                }
                if (index + 1 === data.files.length) {
                    data.context.find('button')
                        .text('Upload')
                        .prop('disabled', !!data.files.error);
                }
            })

            .on('fileuploadprogressall', function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#progress .bar').css(
                    'width',
                    progress + '%'
                );
            })

            .on('fileuploaddone', function (e, data) {
                $.each(data.result.files, function (index, file) {
                    var link = $('<a>')
                        .attr('target', '_blank')
                        .prop('href', file.url);
                    $(data.context.children()[index])
                        .wrap(link);
                });
            })

            .on('fileuploadfail', function (e, data) {
                $.each(data.result.files, function (index, file) {
                    var error = $('<span/>').text(file.error);
                    $(data.context.children()[index])
                        .append('<br>')
                        .append(error);
                });
            })

            .prop('disabled', !$.support.fileInput)
            .parent().addClass($.support.fileInput ? undefined : 'disabled');
        },

        // Bind event handlers in Basic Plus UI theme
        _initBasicPlusUITheme: function($this) {
            $this
            .prop('disabled', !$.support.fileInput)
            .parent().addClass($.support.fileInput ? undefined : 'disabled')
        },


    })
}))
