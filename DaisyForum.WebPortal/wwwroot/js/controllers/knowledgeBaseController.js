var knowledgeBaseController = function () {
    this.initialize = function () {
        var kbId = parseInt($('#hid_knowledge_base_id').val());
        loadComments(kbId);
        registerEvents();
    };

    function registerEvents() {
        // this is the id of the form
        $("#commentform").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.
            var form = $(this);
            var url = form.attr('action');

            $.post(url, form.serialize()).done(function (response) {
                var content = $("#txt_new_comment_content").val();

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

                $('#message-result').removeClass('alert-danger')
                    .addClass('alert-success')
                    .html('Bình luận thành công')
                    .show();
            }).error(function (err) {
                $('#message-result').html('');
                if (err.status === 400 && err.responseText) {
                    var errMsgs = JSON.parse(err.responseText);
                    for (field in errMsgs) {
                        $('#message-result').append(errMsgs[field] + '<br>');
                    }
                    $('#message-result')
                        .removeClass('alert-success"')
                        .addClass('alert-danger')
                        .show();
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

                ClassicEditor
                    .create(document.querySelector('#txt_reply_content_' + commentId))
                    .catch(error => {
                        console.error(error);
                    });
                // this is the id of the form
                $("#frm_reply_comment_" + commentId).submit(function (e) {
                    e.preventDefault(); // avoid to execute the actual submit of the form.
                    var form = $(this);
                    var url = form.attr('action');

                    $.post(url, form.serialize()).done(function (response) {
                        var content = $("#txt_reply_content_" + commentId).val();
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

                        $('#liveToast').addClass('bg-success');
                        $('#message').text('Bình luận thành công');
                        const toastLiveExample = document.getElementById('liveToast')
                        const toast = new bootstrap.Toast(toastLiveExample)
                        toast.show()
                    }).error(function (err) {
                        $('#message-result-reply-' + commentId).html('');
                        if (err.status === 400 && err.responseText) {
                            var errMsgs = JSON.parse(err.responseText);
                            for (field in errMsgs) {
                                $('#message-result-reply-' + commentId).append(errMsgs[field] + '<br>');
                                $('#message').text(errMsgs[field]);
                            }

                            $('#liveToast').addClass('bg-danger');

                            const toastLiveExample = document.getElementById('liveToast')
                            const toast = new bootstrap.Toast(toastLiveExample)
                            toast.show()
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
                ClassicEditor
                    .create(document.querySelector('#txt_reply_content_' + commentId))
                    .catch(error => {
                        console.error(error);
                    });
                // this is the id of the form
                $("#frm_reply_comment_" + commentId).submit(function (e) {
                    e.preventDefault(); // avoid to execute the actual submit of the form.
                    var form = $(this);
                    var url = form.attr('action');

                    $.post(url, form.serialize()).done(function (response) {
                        var content = $("#txt_reply_content_" + commentId).val();
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

                        $('#liveToast').addClass('bg-success');
                        $('#message').text('Bình luận thành công');
                        const toastLiveExample = document.getElementById('liveToast')
                        const toast = new bootstrap.Toast(toastLiveExample)
                        toast.show()
                    }).error(function (err) {
                        $('#message-result-reply-' + commentId).html('');
                        if (err.status === 400 && err.responseText) {
                            var errMsgs = JSON.parse(err.responseText);
                            for (field in errMsgs) {
                                $('#message-result-reply-' + commentId).append(errMsgs[field] + '<br>');
                                $('#message').text(errMsgs[field]);
                            }

                            $('#liveToast').addClass('bg-danger');

                            const toastLiveExample = document.getElementById('liveToast')
                            const toast = new bootstrap.Toast(toastLiveExample)
                            toast.show()
                        }
                    });
                });
            }
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
            loadRepliedComments(kbId, commentId, nextPageIndex);
        });
    }

    // function loadComments(id, pageIndex) {
    //     if (pageIndex === undefined) pageIndex = 1;
    //     $.get('/knowledgeBase/GetCommentsByKnowledgeBaseId?knowledgeBaseId=' + id + '&pageIndex=' + pageIndex)
    //         .done(function (response, statusText, xhr) {
    //             if (xhr.status === 200) {
    //                 var template = $('#tmpl_comments').html();
    //                 var childrenTemplate = $('#tmpl_children_comments').html();
    //                 if (response && response.items) {
    //                     var html = '';
    //                     $.each(response.items, function (index, item) {
    //                         var childrenHtml = '';
    //                         if (item.children && item.children.items) {
    //                             $.each(item.children.items, function (childIndex, childItem) {
    //                                 childrenHtml += Mustache.render(childrenTemplate, {
    //                                     id: childItem.id,
    //                                     content: childItem.content,
    //                                     createDate: formatRelativeTime(childItem.createDate),
    //                                     ownerName: childItem.ownerName
    //                                 });
    //                             });
    //                         }
    //                         if (response.pageIndex > response.pageCount) {
    //                             childrenHtml += '<a href="#" class="replied-comment-pagination" id="replied-comment-pagination-' + item.id + '" data-page-index="1" data-id="' + item.id + '">Xem thêm bình luận</a>';
    //                         }
    //                         else {
    //                             childrenHtml += '<a href="#" class="replied-comment-pagination" id="replied-comment-pagination-' + item.id + '" data-page-index="1" data-id="' + item.id + '" style="display:none">Xem thêm bình luận</a>';
    //                         }

    //                         html += Mustache.render(template, {
    //                             childrenHtml: childrenHtml,
    //                             id: item.id,
    //                             content: item.content,
    //                             createDate: formatRelativeTime(item.createDate),
    //                             ownerName: item.ownerName,
    //                             content: item.content
    //                         });
    //                     });
    //                     $('#comment_list').append(html);
    //                     if (response.pageIndex < response.pageCount) {
    //                         $('#comment-pagination').show();
    //                     }
    //                     else {
    //                         $('#comment-pagination').hide();
    //                     }
    //                 }
    //             }
    //         });
    // }

    function AIComment() {
        var apiKey = 'YOUR_API_KEY_HERE'; // Thay thế bằng API key của bạn
        var apiUrl = 'https://language.googleapis.com/v1/documents:analyzeSentiment?key=' + apiKey;
        var comment = 'Fuck you'; // Thay thế bằng bình luận để phân tích

        // Thiết lập dữ liệu để gửi đến API
        var requestData = {
            document: {
                type: 'PLAIN_TEXT',
                content: comment
            },
            encodingType: 'UTF8'
        };

        // Gửi yêu cầu API bằng jQuery
        $.ajax({
            url: apiUrl,
            type: 'POST',
            data: JSON.stringify(requestData),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response)
                    if (response.documentSentiment.score < 0) {
                        console.log('This is a negative comment');
                        // Do something with the negative comment, such as hide it or display a warning message
                    } else {
                        console.log('This is not a negative comment');
                        // Do something with the non-negative comment, such as display it
                    }
            },
            error: function (error) {
                console.log('Error:', error);
                // Handle the error, such as displaying an error message
            }
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
            repliedCommentPaginationHtml,
        });
    }

    function loadRepliedComments(id, rootCommentId, pageIndex) {
        if (pageIndex === undefined) pageIndex = 1;
        $.get('/knowledgeBase/GetRepliedCommentsByKnowledgeBaseId?knowledgeBaseId=' + id + '&rootcommentId=' + rootCommentId
            + '&pageIndex=' + pageIndex)
            .done(function (response, statusText, xhr) {
                if (xhr.status === 200) {
                    var template = $('#tmpl_children_comments').html();
                    if (response && response.items) {
                        var html = '';
                        $.each(response.items, function (index, item) {
                            html += Mustache.render(template, {
                                id: item.id,
                                content: item.content,
                                createDate: formatRelativeTime(item.createDate),
                                ownerName: item.ownerName,
                                content: item.content
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