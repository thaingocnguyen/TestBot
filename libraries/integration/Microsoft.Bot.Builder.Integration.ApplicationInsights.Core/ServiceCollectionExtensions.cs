﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Bot.Builder.Integration.ApplicationInsights.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures services for Application Insights to the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> which specifies the contract for a collection of service descriptors.</param>
        /// <param name="botConfiguration">Bot configuration that contains the Application Insights configuration information.</param>
        /// <param name="appInsightsServiceInstanceName">(OPTIONAL) Specifies a Application Insights instance name in the Bot configuration.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddBotApplicationInsights(this IServiceCollection services, BotConfiguration botConfiguration, string appInsightsServiceInstanceName = null)
        {
            if (botConfiguration == null)
            {
                throw new ArgumentNullException(nameof(botConfiguration));
            }

            var appInsightsServices = botConfiguration.Services.OfType<AppInsightsService>();

            AppInsightsService appInsightService = null;

            // Validate the bot file is correct.
            var instanceNameSpecified = appInsightsServiceInstanceName != null;
            if (instanceNameSpecified)
            {
                appInsightService = appInsightsServices.Where(ais => ais.Name == appInsightsServiceInstanceName).FirstOrDefault();
                if (appInsightService == null)
                {
                    throw new ArgumentException($"No Application Insights Service instance with the specified name \"{appInsightsServiceInstanceName}\" was found in the {nameof(BotConfiguration)}");
                }
            }
            else
            {
                appInsightService = appInsightsServices.FirstOrDefault();
            }

            CreateBotTelemetry(services);

            IBotTelemetryClient telemetryClient = null;
            if (appInsightService != null)
            {
                services.AddApplicationInsightsTelemetry(appInsightService.InstrumentationKey);
                telemetryClient = new BotTelemetryClient(new TelemetryClient());
            }
            else
            {
                telemetryClient = NullBotTelemetryClient.Instance;
            }

            services.AddSingleton(telemetryClient);

            return services;
        }

        /// <summary>
        /// Adds and configures services for Application Insights to the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> which specifies the contract for a collection of service descriptors.</param>
        /// <param name="botTelemetryClient">Bot Telemetry Client that logs event information.</param>
        /// <param name="instrumentationKey">If Bot Telemetry Client is using Application Insights, provide the instumentation key.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddBotApplicationInsights(this IServiceCollection services, IBotTelemetryClient botTelemetryClient, string instrumentationKey = null)
        {
            if (botTelemetryClient == null)
            {
                throw new ArgumentNullException(nameof(botTelemetryClient));
            }

            CreateBotTelemetry(services);

            // Start Application Insights
            if (instrumentationKey != null)
            {
                services.AddApplicationInsightsTelemetry(instrumentationKey);
            }

            // Register the BotTelemetryClient
            services.AddSingleton(botTelemetryClient);

            return services;

        }

        private static void CreateBotTelemetry(IServiceCollection services)
        {
            // Enables Bot Telemetry to save user/session id's as the bot user id and session
            services.AddTransient<TelemetrySaveBodyASPMiddleware>();
            services.AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>();
        }
    }
}
