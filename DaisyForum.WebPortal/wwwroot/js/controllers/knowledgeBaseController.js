var knowledgeBaseController = function () {
    this.initialize = function () {
        var kbId = parseInt($('#hid_knowledge_base_id').val());
        loadComments(kbId);
        registerEvents();
    };

    function ToastSimple(success, message) {
        if (success) {
            $('#liveToast').addClass('bg-success').addClass('text-white');
            $('#message').text(message);
            const toastLiveExample = document.getElementById('liveToast')
            const toast = new bootstrap.Toast(toastLiveExample)
            toast.show()
        } else {
            $('#liveToast').addClass('bg-danger').addClass('text-white');
            $('#message').text(message);
            const toastLiveExample = document.getElementById('liveToast')
            const toast = new bootstrap.Toast(toastLiveExample)
            toast.show()
        }
    }

    function registerEvents() {
        // this is the id of the form
        $("#commentform").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.
            var form = $(this);
            var url = form.attr('action');
            var content = $("#txt_new_comment_content").val();

            $.post(url, form.serialize()).done(function (response) {
                var template = $('#tmpl_comments').html();
                var newComment = Mustache.render(template, {
                    id: response.id,
                    content: content,
                    createDate: formatRelativeTime(),
                    ownerName: $('#hid_current_login_name').val(),
                });
                $("#txt_new_comment_content").val('');

                $('#comment_list').prepend(newComment);
                var numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                $('#hid_number_comments').val(numberOfComments);
                $('#comments-title').text('(' + numberOfComments + ') bình luận');

                ToastSimple(true, 'Bình luận thành công');
            }).error(function (err) {
                $('#message-result').html('');
                if (err.status === 400 && err.responseText) {
                    var errMsgs = JSON.parse(err.responseText);
                    for (field in errMsgs) {
                        ToastSimple(false, errMsgs[field])
                    }
                }
            });

        });

        //Binding reply comment event
        $('body').on('click', '.comment-reply-link', function (e) {
            e.preventDefault();
            var commentId = $(this).data('commentid');
            var template = $('#tmpl_reply_comment').html();
            var html = Mustache.render(template, {
                commentId: commentId
            });

            var form = $('#reply_comment_' + commentId).html();

            if (form.length > 0) {
                $('#reply_comment_' + commentId).html('');
            }
            else {
                $('#reply_comment_' + commentId).html(html);


                CKEDITOR.ClassicEditor.create(document.getElementById('txt_reply_content_' + commentId), {
                    // https://ckeditor.com/docs/ckeditor5/latest/features/toolbar/toolbar.html#extended-toolbar-configuration-format
                    toolbar: {
                        items: [
                            'exportPDF', 'exportWord', 'findAndReplace', '|',
                            'heading', '|',
                            'bold', 'italic', 'strikethrough', 'underline', 'code', 'subscript', 'superscript', 'removeFormat', '|',
                            'bulletedList', 'numberedList', 'todoList', 'outdent', 'indent', '|',
                            'undo', 'redo',
                            '-',
                            'fontSize', 'fontFamily', 'fontColor', 'fontBackgroundColor', 'highlight', '|',
                            'alignment', '|',
                            'link', 'insertImage', 'blockQuote', 'insertTable', 'mediaEmbed', 'codeBlock', 'htmlEmbed', '|',
                            'specialCharacters', 'horizontalLine', 'pageBreak', '|',
                            'textPartLanguage', '|',
                            'sourceEditing'
                        ],
                        shouldNotGroupWhenFull: true
                    },
                    // Changing the language of the interface requires loading the language file using the <script> tag.
                    // language: 'es',
                    list: {
                        properties: {
                            styles: true,
                            startIndex: true,
                            reversed: true
                        }
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/headings.html#configuration
                    heading: {
                        options: [
                            { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
                            { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
                            { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' },
                            { model: 'heading3', view: 'h3', title: 'Heading 3', class: 'ck-heading_heading3' },
                            { model: 'heading4', view: 'h4', title: 'Heading 4', class: 'ck-heading_heading4' },
                            { model: 'heading5', view: 'h5', title: 'Heading 5', class: 'ck-heading_heading5' },
                            { model: 'heading6', view: 'h6', title: 'Heading 6', class: 'ck-heading_heading6' }
                        ]
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/editor-placeholder.html#using-the-editor-configuration
                    placeholder: 'Nhập nội dung',
                    // https://ckeditor.com/docs/ckeditor5/latest/features/font.html#configuring-the-font-family-feature
                    fontFamily: {
                        options: [
                            'default',
                            'Arial, Helvetica, sans-serif',
                            'Courier New, Courier, monospace',
                            'Georgia, serif',
                            'Lucida Sans Unicode, Lucida Grande, sans-serif',
                            'Tahoma, Geneva, sans-serif',
                            'Times New Roman, Times, serif',
                            'Trebuchet MS, Helvetica, sans-serif',
                            'Verdana, Geneva, sans-serif'
                        ],
                        supportAllValues: true
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/font.html#configuring-the-font-size-feature
                    fontSize: {
                        options: [10, 12, 14, 'default', 18, 20, 22],
                        supportAllValues: true
                    },
                    // Be careful with the setting below. It instructs CKEditor to accept ALL HTML markup.
                    // https://ckeditor.com/docs/ckeditor5/latest/features/general-html-support.html#enabling-all-html-features
                    htmlSupport: {
                        allow: [
                            {
                                name: /.*/,
                                attributes: true,
                                classes: true,
                                styles: true
                            }
                        ]
                    },
                    // Be careful with enabling previews
                    // https://ckeditor.com/docs/ckeditor5/latest/features/html-embed.html#content-previews
                    htmlEmbed: {
                        showPreviews: true
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/link.html#custom-link-attributes-decorators
                    link: {
                        decorators: {
                            addTargetToExternalLinks: true,
                            defaultProtocol: 'https://',
                            toggleDownloadable: {
                                mode: 'manual',
                                label: 'Downloadable',
                                attributes: {
                                    download: 'file'
                                }
                            }
                        }
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/mentions.html#configuration
                    mention: {
                        feeds: [
                            {
                                marker: '@',
                                feed: [
                                    '@apple', '@bears', '@brownie', '@cake', '@cake', '@candy', '@canes', '@chocolate', '@cookie', '@cotton', '@cream',
                                    '@cupcake', '@danish', '@donut', '@dragée', '@fruitcake', '@gingerbread', '@gummi', '@ice', '@jelly-o',
                                    '@liquorice', '@macaroon', '@marzipan', '@oat', '@pie', '@plum', '@pudding', '@sesame', '@snaps', '@soufflé',
                                    '@sugar', '@sweet', '@topping', '@wafer'
                                ],
                                minimumCharacters: 1
                            }
                        ]
                    },
                    // The "super-build" contains more premium features that require additional configuration, disable them below.
                    // Do not turn them on unless you read the documentation and know how to configure them and setup the editor.
                    removePlugins: [
                        // These two are commercial, but you can try them out without registering to a trial.
                        // 'ExportPdf',
                        // 'ExportWord',
                        'CKBox',
                        'CKFinder',
                        'EasyImage',
                        // This sample uses the Base64UploadAdapter to handle image uploads as it requires no configuration.
                        // https://ckeditor.com/docs/ckeditor5/latest/features/images/image-upload/base64-upload-adapter.html
                        // Storing images as Base64 is usually a very bad idea.
                        // Replace it on production website with other solutions:
                        // https://ckeditor.com/docs/ckeditor5/latest/features/images/image-upload/image-upload.html
                        // 'Base64UploadAdapter',
                        'RealTimeCollaborativeComments',
                        'RealTimeCollaborativeTrackChanges',
                        'RealTimeCollaborativeRevisionHistory',
                        'PresenceList',
                        'Comments',
                        'TrackChanges',
                        'TrackChangesData',
                        'RevisionHistory',
                        'Pagination',
                        'WProofreader',
                        // Careful, with the Mathtype plugin CKEditor will not load when loading this sample
                        // from a local file system (file://) - load this site via HTTP server if you enable MathType.
                        'MathType',
                        // The following features are part of the Productivity Pack and require additional license.
                        'SlashCommand',
                        'Template',
                        'DocumentOutline',
                        'FormatPainter',
                        'TableOfContents'
                    ]
                });


                // this is the id of the form
                $("#frm_reply_comment_" + commentId).submit(function (e) {
                    e.preventDefault(); // avoid to execute the actual submit of the form.
                    var form = $(this);
                    var url = form.attr('action');
                    var content = $("#txt_reply_content_" + commentId).val();


                    $.post(url, form.serialize()).done(function (response) {
                        var template = $('#tmpl_children_comments').html();
                        var newComment = Mustache.render(template, {
                            id: commentId,
                            content: content,
                            createDate: formatRelativeTime(),
                            ownerName: $('#hid_current_login_name').val(),
                        });

                        //Reset reply comment
                        $("#txt_reply_content_" + commentId).val('');
                        $('#reply_comment_' + commentId).html('');

                        //Prepend new comment to children
                        $('#children_comments_' + commentId).prepend(newComment);

                        //Update number of comment
                        var numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                        $('#hid_number_comments').val(numberOfComments);
                        $('#comments-title').text('(' + numberOfComments + ') bình luận');
                        ToastSimple(true, 'Bình luận thành công');
                    })
                        .error(function (err) {
                            $('#message-result-reply-' + commentId).html('');
                            if (err.status === 400 && err.responseText) {
                                var errMsgs = JSON.parse(err.responseText);
                                for (field in errMsgs) {
                                    ToastSimple(false, errMsgs[field])
                                }
                            }
                        });
                });
            }
        });


        //Binding reply comment event
        $('body').on('click', '.comment-reply-link_child', function (e) {
            e.preventDefault();
            var commentId = $(this).data('commentid');
            var template = $('#tmpl_reply_comment').html();
            var html = Mustache.render(template, {
                commentId: commentId
            });

            var form = $('#reply_comment_child_' + commentId).html();
            if (form.length > 0) {
                $('#reply_comment_child_' + commentId).html('');
            }
            else {
                $('#reply_comment_child_' + commentId).html(html);
                CKEDITOR.ClassicEditor.create(document.getElementById('txt_reply_content_' + commentId), {
                    // https://ckeditor.com/docs/ckeditor5/latest/features/toolbar/toolbar.html#extended-toolbar-configuration-format
                    toolbar: {
                        items: [
                            'exportPDF', 'exportWord', 'findAndReplace', '|',
                            'heading', '|',
                            'bold', 'italic', 'strikethrough', 'underline', 'code', 'subscript', 'superscript', 'removeFormat', '|',
                            'bulletedList', 'numberedList', 'todoList', 'outdent', 'indent', '|',
                            'undo', 'redo',
                            '-',
                            'fontSize', 'fontFamily', 'fontColor', 'fontBackgroundColor', 'highlight', '|',
                            'alignment', '|',
                            'link', 'insertImage', 'blockQuote', 'insertTable', 'mediaEmbed', 'codeBlock', 'htmlEmbed', '|',
                            'specialCharacters', 'horizontalLine', 'pageBreak', '|',
                            'textPartLanguage', '|',
                            'sourceEditing'
                        ],
                        shouldNotGroupWhenFull: true
                    },
                    // Changing the language of the interface requires loading the language file using the <script> tag.
                    // language: 'es',
                    list: {
                        properties: {
                            styles: true,
                            startIndex: true,
                            reversed: true
                        }
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/headings.html#configuration
                    heading: {
                        options: [
                            { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
                            { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
                            { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' },
                            { model: 'heading3', view: 'h3', title: 'Heading 3', class: 'ck-heading_heading3' },
                            { model: 'heading4', view: 'h4', title: 'Heading 4', class: 'ck-heading_heading4' },
                            { model: 'heading5', view: 'h5', title: 'Heading 5', class: 'ck-heading_heading5' },
                            { model: 'heading6', view: 'h6', title: 'Heading 6', class: 'ck-heading_heading6' }
                        ]
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/editor-placeholder.html#using-the-editor-configuration
                    placeholder: 'Nhập nội dung',
                    // https://ckeditor.com/docs/ckeditor5/latest/features/font.html#configuring-the-font-family-feature
                    fontFamily: {
                        options: [
                            'default',
                            'Arial, Helvetica, sans-serif',
                            'Courier New, Courier, monospace',
                            'Georgia, serif',
                            'Lucida Sans Unicode, Lucida Grande, sans-serif',
                            'Tahoma, Geneva, sans-serif',
                            'Times New Roman, Times, serif',
                            'Trebuchet MS, Helvetica, sans-serif',
                            'Verdana, Geneva, sans-serif'
                        ],
                        supportAllValues: true
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/font.html#configuring-the-font-size-feature
                    fontSize: {
                        options: [10, 12, 14, 'default', 18, 20, 22],
                        supportAllValues: true
                    },
                    // Be careful with the setting below. It instructs CKEditor to accept ALL HTML markup.
                    // https://ckeditor.com/docs/ckeditor5/latest/features/general-html-support.html#enabling-all-html-features
                    htmlSupport: {
                        allow: [
                            {
                                name: /.*/,
                                attributes: true,
                                classes: true,
                                styles: true
                            }
                        ]
                    },
                    // Be careful with enabling previews
                    // https://ckeditor.com/docs/ckeditor5/latest/features/html-embed.html#content-previews
                    htmlEmbed: {
                        showPreviews: true
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/link.html#custom-link-attributes-decorators
                    link: {
                        decorators: {
                            addTargetToExternalLinks: true,
                            defaultProtocol: 'https://',
                            toggleDownloadable: {
                                mode: 'manual',
                                label: 'Downloadable',
                                attributes: {
                                    download: 'file'
                                }
                            }
                        }
                    },
                    // https://ckeditor.com/docs/ckeditor5/latest/features/mentions.html#configuration
                    mention: {
                        feeds: [
                            {
                                marker: '@',
                                feed: [
                                    '@apple', '@bears', '@brownie', '@cake', '@cake', '@candy', '@canes', '@chocolate', '@cookie', '@cotton', '@cream',
                                    '@cupcake', '@danish', '@donut', '@dragée', '@fruitcake', '@gingerbread', '@gummi', '@ice', '@jelly-o',
                                    '@liquorice', '@macaroon', '@marzipan', '@oat', '@pie', '@plum', '@pudding', '@sesame', '@snaps', '@soufflé',
                                    '@sugar', '@sweet', '@topping', '@wafer'
                                ],
                                minimumCharacters: 1
                            }
                        ]
                    },
                    // The "super-build" contains more premium features that require additional configuration, disable them below.
                    // Do not turn them on unless you read the documentation and know how to configure them and setup the editor.
                    removePlugins: [
                        // These two are commercial, but you can try them out without registering to a trial.
                        // 'ExportPdf',
                        // 'ExportWord',
                        'CKBox',
                        'CKFinder',
                        'EasyImage',
                        // This sample uses the Base64UploadAdapter to handle image uploads as it requires no configuration.
                        // https://ckeditor.com/docs/ckeditor5/latest/features/images/image-upload/base64-upload-adapter.html
                        // Storing images as Base64 is usually a very bad idea.
                        // Replace it on production website with other solutions:
                        // https://ckeditor.com/docs/ckeditor5/latest/features/images/image-upload/image-upload.html
                        // 'Base64UploadAdapter',
                        'RealTimeCollaborativeComments',
                        'RealTimeCollaborativeTrackChanges',
                        'RealTimeCollaborativeRevisionHistory',
                        'PresenceList',
                        'Comments',
                        'TrackChanges',
                        'TrackChangesData',
                        'RevisionHistory',
                        'Pagination',
                        'WProofreader',
                        // Careful, with the Mathtype plugin CKEditor will not load when loading this sample
                        // from a local file system (file://) - load this site via HTTP server if you enable MathType.
                        'MathType',
                        // The following features are part of the Productivity Pack and require additional license.
                        'SlashCommand',
                        'Template',
                        'DocumentOutline',
                        'FormatPainter',
                        'TableOfContents'
                    ]
                });
                // this is the id of the form
                $("#frm_reply_comment_" + commentId).submit(function (e) {
                    e.preventDefault(); // avoid to execute the actual submit of the form.
                    var form = $(this);
                    var url = form.attr('action');
                    var content = $("#txt_reply_content_" + commentId).val();

                    $.post(url, form.serialize()).done(function (response) {

                        var template = $('#tmpl_children_comments').html();
                        var newComment = Mustache.render(template, {
                            id: commentId,
                            content: content,
                            createDate: formatRelativeTime(),
                            ownerName: $('#hid_current_login_name').val(),
                        });

                        //Reset reply comment
                        $("#txt_reply_content_" + commentId).val('');
                        $('#reply_comment_' + commentId).html('');

                        //Prepend new comment to children
                        $('#children_comments_' + commentId).prepend(newComment);

                        //Update number of comment
                        var numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                        $('#hid_number_comments').val(numberOfComments);
                        $('#comments-title').text('(' + numberOfComments + ') bình luận');

                        ToastSimple(true, 'Bình luận thành công');
                    }).error(function (err) {
                        $('#message-result-reply-' + commentId).html('');
                        if (err.status === 400 && err.responseText) {
                            var errMsgs = JSON.parse(err.responseText);
                            for (field in errMsgs) {
                                $('#message-result-reply-' + commentId).append(errMsgs[field] + '<br>');
                                ToastSimple(false, errMsgs[field]);
                            }
                        }
                    });
                });
            }
        });

        $(document).on('click', '[id^="delete_"]', function (e) {
            e.preventDefault();
            var commentId = $(this).attr('id').replace('delete_', '');

            // Hiển thị modal xác nhận xóa bình luận
            $('#confirmDeleteModal').modal('show');

            // Gán commentId vào data của nút Xóa trong modal
            $('#confirmDeleteButton').data('comment-id', commentId);
        });

        // Xử lý sự kiện click vào nút Xóa trong modal
        $('#confirmDeleteButton').on('click', function () {
            var commentId = $(this).data('comment-id');
            var knowledgeBaseId = parseInt($('#hid_knowledge_base_id').val());

            // Gọi API xóa bình luận tương ứng
            $.ajax({
                url: '/knowledgeBase/DeleteComment',
                type: 'DELETE',
                data: { knowledgeBaseId: knowledgeBaseId, commentId: commentId },
                success: function (result) {
                    ToastSimple(true, "Xóa bình luận thành công");
                    $('#comment_' + commentId).remove();
                },
                error: function (xhr, statusText, error) {
                    ToastSimple(false, "Xóa bình luận thất bại");
                }
            });

            // Ẩn modal
            $('#confirmDeleteModal').modal('hide');
        });

        $('#frm_vote').submit(function (e) {
            e.preventDefault();
            var form = $(this);
            $.post('/knowledgeBase/postVote', form.serialize()).done(function (response) {
                $('#like-num').text(response);
                $('#like-count').text(response);
            });
        });
        $('#frm_vote .like-it').click(function () {
            $('#frm_vote').submit();
        });

        $('#btn_send_report').off('click').on('click', function (e) {
            e.preventDefault();
            var form = $('#frm_report');
            $.post('/knowledgeBase/postReport', form.serialize())
                .done(function () {
                    $('#reportModal').modal('hide');
                    $('#txt_report_content').val('');
                }).error(function (err) {
                    $('#message-result-report').html('');
                    if (err.status === 400 && err.responseText) {
                        var errMsgs = JSON.parse(err.responseText);
                        for (field in errMsgs) {
                            $('#message-result-report').append(errMsgs[field] + '<br>');
                        }
                        $('#message-result-report')
                            .removeClass('alert-success"')
                            .addClass('alert-danger')
                            .show();
                    }
                });
        });

        $('body').on('click', '#comment-pagination', function (e) {
            e.preventDefault();
            var kbId = parseInt($('#hid_knowledge_base_id').val());
            var nextPageIndex = parseInt($(this).data('page-index')) + 1;
            $(this).data('page-index', nextPageIndex);
            loadComments(kbId, nextPageIndex);
        });

        $('body').on('click', '.replied-comment-pagination', function (e) {
            e.preventDefault();
            var kbId = parseInt($('#hid_knowledge_base_id').val());

            var commentId = parseInt($(this).data('id'));
            var nextPageIndex = parseInt($(this).data('page-index')) + 1;
            $(this).data('page-index', nextPageIndex);
            var currentUserId = $('#hid_current_id').val()
            loadRepliedComments(kbId, commentId, nextPageIndex, currentUserId);
        });
    }

    function loadComments(id, pageIndex) {
        if (pageIndex === undefined) {
            pageIndex = 1;
        }
        $.get(`/knowledgeBase/GetCommentsByKnowledgeBaseId?knowledgeBaseId=${id}&pageIndex=${pageIndex}`)
            .done(function (response, statusText, xhr) {
                if (xhr.status === 200) {
                    const template = $('#tmpl_comments').html();

                    if (response?.items) {
                        let html = '';
                        response.items.forEach((item) => {
                            html += generateCommentHtml(item, template);
                        });

                        $('#comment_list').append(html);

                        if (response.pageIndex < response.pageCount) {
                            $('#comment-pagination').show();
                        } else {
                            $('#comment-pagination').hide();
                        }
                    }
                }
            });
    }

    function generateCommentHtml(comment, template) {
        let childrenHtml = '';
        if (comment.children?.items) {
            comment.children.items.forEach((childComment) => {
                childrenHtml += generateCommentHtml(childComment, $('#tmpl_children_comments').html());
            });
        }

        let repliedCommentPaginationHtml = '';
        if (comment.children?.pageCount > 1) {
            repliedCommentPaginationHtml = `<a href="#" class="replied-comment-pagination" id="replied-comment-pagination-${comment.id}" data-page-index="1" data-id="${comment.id}">Xem thêm bình luận</a>`;
        }

        return Mustache.render(template, {
            childrenHtml,
            id: comment.id,
            content: comment.content,
            createDate: formatRelativeTime(comment.createDate),
            ownerName: comment.ownerName,
            note: comment.note,
            repliedCommentPaginationHtml,
        });
    }

    function loadRepliedComments(id, rootCommentId, pageIndex, currentUserId) {
        if (pageIndex === undefined) pageIndex = 1;
        $.get('/knowledgeBase/GetRepliedCommentsByKnowledgeBaseId?knowledgeBaseId=' + id + '&rootcommentId=' + rootCommentId
            + '&pageIndex=' + pageIndex)
            .done(function (response, statusText, xhr) {
                if (xhr.status === 200) {
                    var template = $('#tmpl_children_comments').html();
                    if (response && response.items) {
                        var html = '';
                        $.each(response.items, function (index, item) {
                            var isCurrentUserComment = currentUserId === item.ownerUserId;
                            var deleteButtonHtml = isCurrentUserComment ? '<a class="comment-reply-link h6 ml-3 text-danger" style="font-weight: 500" href="#" id="delete_' + item.id + '">Xóa bình luận</a>' : '';

                            html += Mustache.render(template, {
                                id: item.id,
                                content: item.content,
                                createDate: formatRelativeTime(item.createDate),
                                ownerName: item.ownerName,
                                content: item.content,
                                deleteButtonHtml: deleteButtonHtml
                            });
                        });
                        $('#children_comments_' + rootCommentId).append(html);
                        if (response.pageIndex < response.pageCount) {
                            $('#replied-comment-pagination-' + rootCommentId).show();
                        }
                        else {
                            $('#replied-comment-pagination-' + rootCommentId).hide();
                        }
                    }
                }
            });
    }
};