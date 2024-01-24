using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Helpers
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddPage<T>(this IServiceCollection serviceCollection) where T : ContentPage
        {
            serviceCollection.AddTransient(PageWithDisposableContext<T>);
        }

        private static T PageWithDisposableContext<T>(IServiceProvider serviceProvider) where T : ContentPage
        {
            var page = ActivatorUtilities.CreateInstance<T>(serviceProvider);

            page.NavigatingFrom += (object sender, NavigatingFromEventArgs e) =>
            {
                if ((((ContentPage)sender).BindingContext as ILeaving) == null)
                    return;
                (((ContentPage)sender).BindingContext as ILeaving)?.OnLeaving();
            };
            page.Unloaded += (object sender, EventArgs e) =>
            {
                if ((((ContentPage)sender).BindingContext as IDisposable) == null)
                    return;
                (((ContentPage)sender).BindingContext as IDisposable)?.Dispose();
            };

            return page;
        }
    }
}
