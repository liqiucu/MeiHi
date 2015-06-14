$(function () {
    $.validator.addClassRules({
        existsemail: {
            required: true,
            email: true,
            remote: function () {
                var r = {
                    url: "/register/existsemail",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: "{email:'" + $('#existsemail').val() + "'}",
                    dataFilter: function (data) {
                        return $.parseJSON(data);
                    }
                }
                return r;
            }
        },
        registeremail: {
            required: true,
            email: true,
            remote: function () {
                var r = {
                    url: "/register/existsemail",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({ email: $('#registeremail').val() }),
                    dataFilter: function (data) {
                        return !$.parseJSON(data);
                    }
                }
                return r;
            }
        },
        registeradminemail: {
            required: true,
            email: true,
            remote: function () {
                var r = {
                    url: "/role/existsemail",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({ email: $('#registeradminemail').val() }),
                    dataFilter: function (data) {
                        return !$.parseJSON(data);
                    }
                }
                return r;
            }
        },
        password: {
            required: true,
            rangelength: [6, 20]
        },
        confirmemail: {
            equalTo: "#registeremail"
        },
        confirmpassword: {
            equalTo: "#password"
        },
        newpassword: {
            required: true,
            rangelength: [6, 20]
        },
        confirmnewpassword: {
            equalTo: "#newpassword"
        }
    });

    $('form').each(function () {
        $(this).validate({
            messages: {
                username: {
                    required: "请输入用户名"
                },
                email: {
                    required: "请输入邮箱"
                },
                selectedregionid: {
                    required: "请选择城市"
                },
                existsemail: {
                    required: "请输入邮箱",
                    remote: "邮箱地址不存在"
                },
                registeremail: {
                    required: "请输入邮箱",
                    remote: "邮箱地址已被占用"
                },
                registeradminemail: {
                    required: "请输入邮箱",
                    remote: "邮箱地址已被占用"
                },
                mobile: {
                    required: "请输入手机号"
                },
                confirmemail: {
                    equalTo: "两次输入的邮箱不一致"
                },
                password: {
                    required: "请输入密码",
                    rangelength: "密码长度应在{0}和{1}之间"
                },
                confirmpassword: {
                    equalTo: "两次输入的密码不一致"
                },
                newpassword: {
                    required: "请输入新密码",
                    rangelength: "密码长度应在{0}和{1}之间"
                },
                confirmnewpassword: {
                    equalTo: "两次输入的密码不一致"
                },
                terms: {
                    required: "请您必须接受网站的使用条款"
                },
                contact: {
                    required: "请输入联系方式"
                },
                unblockreason: {
                    required: "请输入解封理由"
                },
            },
            errorPlacement: function (label, element) {
                if (element.attr("type") === "checkbox" || element.attr("type") === "radio") {
                    element.parent().append(label); // this would append the label after all your checkboxes/labels (so the error-label will be the last element in <div class="controls"> )
                } else {
                    label.insertAfter(element); // standard behaviour
                }
            },
            success: function (element) {
                $(element).addClass("valid").append("");
            },
            //use the default handler, so comment the bottom line as it will conflict with Obout Editor, which sucks me.
            submitHandler: function (form) {
                form.submit();
            }
        });
    });

});

