﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// Result returned to the caller of one of the various stack manipulation methods,
    /// and used to return the result from a final call to `DialogContext.EndDialogAsync()` to the bot's logic.
    /// </summary>
    public class DialogTurnResult
    {
        public DialogTurnResult(DialogTurnStatus status, object result = null)
        {
            Status = status;
            Result = result;
        }

        /// <summary>
        /// Gets or sets the current status of the stack.
        /// </summary>
        /// <value>
        /// The current status of the stack.
        /// </value>
        public DialogTurnStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the result returned by a dialog that was just ended.
        /// This will only be populated in certain cases:
        ///
        /// - The bot calls `DialogContext.BeginDialogAsync()` to start a new dialog and the dialog ends immediately.
        /// - The bot calls `DialogContext.ContinueDialogAsync()` and a dialog that was active ends.
        ///
        /// In all cases where it's populated, <see cref="DialogContext.ActiveDialog"/> will be `null`.
        /// </summary>
        /// <value>
        /// The result returned by a dialog that was just ended.
        /// </value>
        public object Result { get; set; }
    }
}
