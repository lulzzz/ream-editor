// json interfaces for query backend api
enum StatusCode {
    Ok = 0,
    CompilationError,
    ServerUnreachable,
    ConnectionStringSyntax,
    NamespaceIdentifier,
    UnknownError   
}

type databaseProviderType = 'SqlServer' | 'Npgsql' | 'Sqlite';

export interface BaseRequest { id: string; }

export interface BaseResponse { code: string; message: string; }

export interface CodeRequest extends BaseRequest {
    text: string;
}

export interface QueryTemplateRequest extends BaseRequest {
    serverType: databaseProviderType;
    connectionString: string;
    text: string;
}

export interface CodeTemplateRequest extends BaseRequest {
    text: string;
}

export interface CodeTemplateResponse extends BaseResponse {
        namespace: string;
        template: string;
        header: string;
        footer: string;
        columnOffset: number;
        lineOffset: number;
        defaultQuery: string;
}

// omnisharp api
export interface UpdateBufferRequest {
    SessionId: string; // not used by omnisharp, internally used field
    FileName: string;
    FromDisk: boolean;
    Buffer: string;
}

export interface AutoCompletionItem {
    CompletionText: string;
    Description: string;
    DisplayText: string;
    RequiredNamespaceImport: string;
    MethodHeader: string;
    ReturnType: string;
    Snippet: string;
    Kind: string;
}