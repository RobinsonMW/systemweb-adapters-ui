//MIT license

using System.Collections;
using System.ComponentModel;

namespace System.Web.UI.WebControls;

public class DetailsViewRowsGenerator : AutoFieldsGenerator {

    public override List<AutoGeneratedField> CreateAutoGeneratedFields(object dataItem, Control control) {
        if (!(control is DetailsView)) {
            throw new ArgumentException(SR.GetString(SR.InvalidDefaultAutoFieldGenerator, GetType().FullName, typeof(DetailsView).FullName));
        }

        DetailsView detailsView = control as DetailsView;

        if (dataItem == null) {
            // note that we're not throwing an exception in this case, and the calling
            // code should be able to handle a null arraylist being returned
            return null;
        }

        List<AutoGeneratedField> generatedFields = new List<AutoGeneratedField>();
        PropertyDescriptorCollection propDescs = null;
        bool throwException = true;
        Type dataItemType = null;

        //The base class ensures that the AutoGeneratedFieldProperties collection is reset before this method is called.
        //However we are doing this again in here because we have another caller DetailsView.CreateAutoGeneratedRows which is
        //not being used anywhere today but kept for backward compatibility.
        if (AutoGeneratedFieldProperties.Count > 0) {
            AutoGeneratedFieldProperties.Clear();
        }

        if (dataItem != null)
            dataItemType = dataItem.GetType();

        if ((dataItem != null) && (dataItem is ICustomTypeDescriptor)) {
            // Get the custom properties of the object
            propDescs = TypeDescriptor.GetProperties(dataItem);
        }
        else if (dataItemType != null) {
            // directly bindable types: strings, ints etc. get treated specially, since we
            // don't care about their properties, but rather we care about them directly
            if (ShouldGenerateField(dataItemType, detailsView)) {
                AutoGeneratedFieldProperties fieldProps = new AutoGeneratedFieldProperties();
                ((IStateManager)fieldProps).TrackViewState();

                fieldProps.Name = "Item";
                fieldProps.DataField = AutoGeneratedField.ThisExpression;
                fieldProps.Type = dataItemType;

                AutoGeneratedField field = CreateAutoGeneratedFieldFromFieldProperties(fieldProps);
                if (field != null) {
                    generatedFields.Add(field);
                    AutoGeneratedFieldProperties.Add(fieldProps);
                }

            }
            else {
                // complex type... we get its properties
                propDescs = TypeDescriptor.GetProperties(dataItemType);
            }
        }

        if ((propDescs != null) && (propDescs.Count != 0)) {
            string[] dataKeyNames = detailsView.DataKeyNames;
            int keyNamesLength = dataKeyNames.Length;
            string[] dataKeyNamesCaseInsensitive = new string[keyNamesLength];
            for (int i = 0; i < keyNamesLength; i++) {
                dataKeyNamesCaseInsensitive[i] = dataKeyNames[i].ToLowerInvariant();
            }
            foreach (PropertyDescriptor pd in propDescs) {
                Type propertyType = pd.PropertyType;
                if (ShouldGenerateField(propertyType, detailsView)) {
                    string name = pd.Name;
                    bool isKey = ((IList)dataKeyNamesCaseInsensitive).Contains(name.ToLowerInvariant());
                    AutoGeneratedFieldProperties fieldProps = new AutoGeneratedFieldProperties();
                    ((IStateManager)fieldProps).TrackViewState();

                    fieldProps.Name = name;
                    fieldProps.IsReadOnly = isKey;
                    fieldProps.Type = propertyType;
                    fieldProps.DataField = name;

                    AutoGeneratedField field = CreateAutoGeneratedFieldFromFieldProperties(fieldProps);
                    if (field != null) {
                        generatedFields.Add(field);
                        AutoGeneratedFieldProperties.Add(fieldProps);
                    }
                }
            }
        }

        if ((generatedFields.Count == 0) && throwException) {
            // this handles the case where we got back something that either had no
            // properties, or all properties were not bindable.
            throw new InvalidOperationException(SR.GetString(SR.DetailsView_NoAutoGenFields, detailsView.ID));
        }

        return generatedFields;
    }

    private bool ShouldGenerateField(Type propertyType, DetailsView detailsView) {
        if (detailsView.RenderingCompatibility < VersionUtil.Framework45 && AutoGenerateEnumFields == null) {
            //This is for backward compatibility. Before 4.5, auto generating fields used to call into this method
            //and if someone has overriden this method to force generation of columns, the scenario should still
            //work.
            return detailsView.IsBindableType(propertyType);
        }
        else {
            //If AutoGenerateEnumFileds is null here, the rendering compatibility must be 4.5
            return DataBoundControlHelper.IsBindableType(propertyType, enableEnums: AutoGenerateEnumFields ?? true);
        }
    }

}