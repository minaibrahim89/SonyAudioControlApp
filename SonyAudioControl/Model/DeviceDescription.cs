using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SonyAudioControl.Model
{
    [XmlRoot(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class SpecVersion
    {
        [XmlElement(ElementName = "major", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Major { get; set; }
        [XmlElement(ElementName = "minor", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Minor { get; set; }
    }

    [XmlRoot(ElementName = "icon", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class Icon
    {
        [XmlElement(ElementName = "mimetype", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Mimetype { get; set; }
        [XmlElement(ElementName = "width", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Width { get; set; }
        [XmlElement(ElementName = "height", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Height { get; set; }
        [XmlElement(ElementName = "depth", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Depth { get; set; }
        [XmlElement(ElementName = "url", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "iconList", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class IconList
    {
        [XmlElement(ElementName = "icon", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public List<Icon> Icons { get; set; }
    }

    [XmlRoot(ElementName = "service", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class Service
    {
        [XmlElement(ElementName = "serviceType", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string ServiceType { get; set; }
        [XmlElement(ElementName = "serviceId", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string ServiceId { get; set; }
        [XmlElement(ElementName = "SCPDURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string SCPDURL { get; set; }
        [XmlElement(ElementName = "controlURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string ControlURL { get; set; }
        [XmlElement(ElementName = "eventSubURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string EventSubURL { get; set; }
    }

    [XmlRoot(ElementName = "serviceList", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class ServiceList
    {
        [XmlElement(ElementName = "service", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public List<Service> Service { get; set; }
    }

    [XmlRoot(ElementName = "X_ScalarWebAPI_ServiceList", Namespace = "urn:schemas-sony-com:av")]
    public class X_ScalarWebAPI_ServiceList
    {
        [XmlElement(ElementName = "X_ScalarWebAPI_ServiceType", Namespace = "urn:schemas-sony-com:av")]
        public List<string> X_ScalarWebAPI_ServiceType { get; set; }
    }

    [XmlRoot(ElementName = "X_ScalarWebAPI_DeviceInfo", Namespace = "urn:schemas-sony-com:av")]
    public class X_ScalarWebAPI_DeviceInfo
    {
        [XmlElement(ElementName = "X_ScalarWebAPI_Version", Namespace = "urn:schemas-sony-com:av")]
        public string X_ScalarWebAPI_Version { get; set; }
        [XmlElement(ElementName = "X_ScalarWebAPI_BaseURL", Namespace = "urn:schemas-sony-com:av")]
        public string X_ScalarWebAPI_BaseURL { get; set; }
        [XmlElement(ElementName = "X_ScalarWebAPI_ServiceList", Namespace = "urn:schemas-sony-com:av")]
        public X_ScalarWebAPI_ServiceList X_ScalarWebAPI_ServiceList { get; set; }
    }

    [XmlRoot(ElementName = "X_CIS_v1Info", Namespace = "urn:schemas-sony-com:av")]
    public class X_CIS_v1Info
    {
        [XmlElement(ElementName = "X_CIS_Port", Namespace = "urn:schemas-sony-com:av")]
        public string X_CIS_Port { get; set; }
    }

    [XmlRoot(ElementName = "X_CIS_v2Info", Namespace = "urn:schemas-sony-com:av")]
    public class X_CIS_v2Info
    {
        [XmlElement(ElementName = "X_CIS_Port", Namespace = "urn:schemas-sony-com:av")]
        public string X_CIS_Port { get; set; }
    }

    [XmlRoot(ElementName = "X_CIS_DeviceInfo", Namespace = "urn:schemas-sony-com:av")]
    public class X_CIS_DeviceInfo
    {
        [XmlElement(ElementName = "X_CIS_Version", Namespace = "urn:schemas-sony-com:av")]
        public string X_CIS_Version { get; set; }
        [XmlElement(ElementName = "X_CIS_v1Info", Namespace = "urn:schemas-sony-com:av")]
        public X_CIS_v1Info X_CIS_v1Info { get; set; }
        [XmlElement(ElementName = "X_CIS_v2Info", Namespace = "urn:schemas-sony-com:av")]
        public X_CIS_v2Info X_CIS_v2Info { get; set; }
    }

    [XmlRoot(ElementName = "device", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class Device
    {
        [XmlElement(ElementName = "deviceType", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string DeviceType { get; set; }
        [XmlElement(ElementName = "friendlyName", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string FriendlyName { get; set; }
        [XmlElement(ElementName = "manufacturer", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string Manufacturer { get; set; }
        [XmlElement(ElementName = "manufacturerURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string ManufacturerURL { get; set; }
        [XmlElement(ElementName = "modelName", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string ModelName { get; set; }
        [XmlElement(ElementName = "modelNumber", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string ModelNumber { get; set; }
        [XmlElement(ElementName = "UDN", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public string UDN { get; set; }
        [XmlElement(ElementName = "X_DLNACAP", Namespace = "urn:schemas-dlna-org:device-1-0")]
        public string X_DLNACAP { get; set; }
        [XmlElement(ElementName = "X_DLNADOC", Namespace = "urn:schemas-dlna-org:device-1-0")]
        public string X_DLNADOC { get; set; }
        [XmlElement(ElementName = "X_StandardDMR", Namespace = "urn:schemas-sony-com:av")]
        public string X_StandardDMR { get; set; }
        [XmlElement(ElementName = "iconList", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public IconList IconList { get; set; }
        [XmlElement(ElementName = "serviceList", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public ServiceList ServiceList { get; set; }
        [XmlElement(ElementName = "X_ScalarWebAPI_DeviceInfo", Namespace = "urn:schemas-sony-com:av")]
        public X_ScalarWebAPI_DeviceInfo X_ScalarWebAPI_DeviceInfo { get; set; }
        [XmlElement(ElementName = "X_CIS_DeviceInfo", Namespace = "urn:schemas-sony-com:av")]
        public X_CIS_DeviceInfo X_CIS_DeviceInfo { get; set; }
    }

    [XmlRoot(ElementName = "root", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class DeviceDescription
    {
        [XmlIgnore]
        public Uri DescriptionLocation { get; set; }

        [XmlElement(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public SpecVersion SpecVersion { get; set; }
        [XmlElement(ElementName = "device", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public Device Device { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "dlna", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dlna { get; set; }
        [XmlAttribute(AttributeName = "av", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Av { get; set; }
    }
}
