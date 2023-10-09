/// <reference path="mscrm.d.ts" />
declare module CustomConnector {
    interface IRecordSource {
        init: (context: Mscrm.ControlData<IRecordSourceParams>, config?: JSON) => Promise<void>;
        getRecordSourceInfo: () => IRecordSourceInfo;
        getRecordsData: (request: IRecordsDataRequest, filter?: IFilterRequest) => Promise<IRecordsDataResponse>;
        getFilterDetails?: (filter?: IFilterRequest) => Promise<IFilterGroup[]>;
        getRecordUX: (recordData: IRecordData, request: IRecordUXRequest) => IRecordUX;
        getRecordCreate?: () => IRecordCreate[];
    }
    interface IRecordCreate {
	id: string;
	iconType: number;
	label: string;
	onClick?: (event: Event) => Promise<void | IRecordCreateResponse>;
    }
    interface IRecordCreateResponse {
	success?: boolean;
	needRefresh?: boolean;
	errorMessage?: string;
    }
    interface IRecordSourceParams {
        controlId: string;
        tableContext?: {
            name: string;
            id: string;
        };
        getTwoLetterInitialsFromUserName: (userName: string) => string;
        refetchRerenderRecords: () => void;
        sanitizeContent: (content: string) => string;
    }
    interface IRecordSourceInfo {
        name: string;
    }
    interface IRecordsDataRequest {
        requestId: string;
        tableContext?: {
            name: string;
            id: string;
        };
        pageSize: number;
        isAscending: boolean;
        lastRefetchedTime: Date;
        lastItem?: IRecordData;
        continuationToken?: string;
    }
    interface IFilterRequest {
        filterData: IFilterGroup[];
        searchKey?: string;
    }
    interface IRecordsDataResponse {
        requestId: string;
        records: IRecordData[];
        continuationToken?: string;
        filtersData?: IFilterGroup[];
    }
    interface IFilterGroup {
        name: string | FilterGroupName;
        label?: string;
        type: FilterGroupType;
        isExpanded?: boolean;
        options?: IFilterOption[];
    }
    const enum FilterGroupType {
        MultiSelect = "MultiSelect",
        SingleSelect = "SingleSelect"
    }
    const enum FilterGroupName {
        RecordType = "Module",
        ActivityType = "ActivityType",
        ActivityStatus = "ActivityStatus",
        ActivityStatusReason = "StatusReason",
        ActivityDue = "Due",
        PostSource = "SystemVsUsersPosts",
        ModifiedDate = "TLG"
    }
    interface IFilterOption {
        value: string | RecordTypeGroupOption | ActivityTypeGroupOption | ActivityStatusGroupOption | ActivityDueGroupOption | PostSourceGroupOption | ModifiedDateGroupOption | ActivityStatusReasonGroupOption;
        label?: string;
        count?: number;
        isSelected?: boolean;
    }
    const enum RecordTypeGroupOption {
        Notes = "Notes",
        Posts = "Posts",
        Activities = "Activities"
    }
    const enum ActivityTypeGroupOption {
        Appointment = "4201",
        CampaignActivity = "4402",
        CaseResolution = "4206",
        Email = "4202",
        Fax = "4204",
        Letter = "4207",
        OpportunityClose = "4208",
        OrderClose = "4209",
        PhoneCall = "4210",
        QuoteClose = "4211",
        RecurringAppointment = "4251",
        SocialActivity = "4216",
        Task = "4212",
        CustomerVoiceAlert = "10295",
        CustomerVoiceSurveyInvite = "10305",
        CustomerVoiceSurveyResponse = "10307"
    }
    const enum ActivityStatusGroupOption {
        Active = "ActiveAndNotOverdueActivityState",
        Overdue = "Overdue",
        Closed = "Closed"
    }
    const enum ActivityDueGroupOption {
        Next30Days = "Next30Days",
        Next7Days = "Next7Days",
        Next24Hours = "Next24Hours",
        Last24Hours = "Last24Hours",
        Last7Days = "Last7Days",
        Last30Days = "Last30Days"
    }
    const enum PostSourceGroupOption {
        AutoPost = "System",
        UserPost = "Users"
    }
    const enum ModifiedDateGroupOption {
        Last24Hours = "Last24Hours",
        Last7Days = "Last7Days",
        Last30Days = "Last30Days"
    }
    const enum ActivityStatusReasonGroupOption {
        Free = "2198156",
        Tentative = "683145742",
        Completed = "601036331",
        Canceled = "-58529607",
        Busy = "2082329",
        OutOfOffice = "-164213037",
        Pending = "982065527",
        InProgress = "-1115514168",
        Aborted = "469875631",
        Proposed = "-928198778",
        Closed = "2021313932",
        SystemAborted = "2106128094",
        Open = "2464362",
        Draft = "66292097",
        Sent = "2573240",
        Received = "-744075775",
        PendingSend = "268330161",
        Sending = "-650390726",
        Failed = "2096857181",
        Scheduled = "1843257485",
        Made = "2390325",
        Requested = "-1597065394",
        Reserved = "-285741240",
        Arrived = "930446413",
        NoShow = "-579192324",
        Processing = "-1879307469",
        NotStarted = "1725055988",
        WaitingOnSomeoneElse = "-1784277931",
        Deferred = "712535039"
    }
    interface IRecordData {
        id: string;
        sortDateValue?: string;
        data: string;
    }
    interface IRecordUXRequest {
        isExpanded: boolean;
    }
    interface IRecordField {
        components: Mscrm.Component[];
    }
    interface IRecordIconData {
        type: number;
        accessibleName: string;
    }
    interface IRecordPersonaBubbleData {
        initials?: string;
        accessibleName: string;
    }
    interface IRecordUX {
        id: string;
        profilePicture?: IRecordPersonaBubbleData;
        icon: IRecordIconData;
        bubbleIcon?: Mscrm.Component;
        header: IRecordField;
        headerIcon?: Mscrm.Component;
        body: IRecordField;
        footer: IRecordField;
        commands?: IRecordCommand[];
        accessibleName: string;
        moduleName: string;
        sortDateValue?: string;
    }
    interface IRecordCommand {
        iconType: number;
        title?: string;
        label: string;
        onClick?: Function;
        command: string;
        commandType: "BUTTON" | "HYPERLINK";
        href?: string;
    }
}
