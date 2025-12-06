namespace Application.Enum
{
    public static class EmailTemplate
    {
        public static string BuildResetPasswordEmail(string resetLink)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Reset Password</title>
                <style>
                    .container {{
                        max-width: 600px;
                        margin: auto;
                        padding: 20px;
                        font-family: Arial, Helvetica, sans-serif;
                        background-color: #f5f5f5;
                        border-radius: 8px;
                    }}

                    .header {{
                        font-size: 22px;
                        font-weight: bold;
                        margin-bottom: 20px;
                        text-align: center;
                    }}

                    .content {{
                        font-size: 15px;
                        color: #333;
                        line-height: 1.6;
                    }}

                    .btn {{
                        display: inline-block;
                        margin-top: 20px;
                        padding: 12px 20px;
                        background-color: #007bff;
                        color: white !important;
                        text-decoration: none;
                        border-radius: 6px;
                        font-size: 16px;
                    }}

                    .footer {{
                        margin-top: 30px;
                        font-size: 13px;
                        color: #777;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>Reset Your Password</div>
        
                    <div class='content'>
                        <p>Hello,</p>
                        <p>You requested a password reset for your account.</p>
                        <p>Click the button below to reset your password:</p>

                        <a class='btn' href='{resetLink}' target='_blank'>Reset Password</a>

                        <p>If the button doesn’t work, copy and paste this link into your browser:</p>
                        <p><a href='{resetLink}'>{resetLink}</a></p>

                        <p>If you didn’t request this, you can safely ignore this email.</p>
                    </div>

                    <div class='footer'>
                        © {DateTime.Now.Year} Your Company — All rights reserved.
                    </div>
                </div>
            </body>
            </html>";
        }
    }

}
